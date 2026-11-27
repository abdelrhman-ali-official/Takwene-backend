using Domain.Contracts;
using Domain.Entities.MusicEntities;
using Domain.Exceptions;
using Services.Abstractions;
using Services.Specifications;
using Shared;
using Shared.ArtistModels;
using Shared.TrackModels;
using System.Text.RegularExpressions;

namespace Services;

public class TrackService(IUnitOFWork unitOfWork) : ITrackService
{
    public Task<bool> IsIsrcAvailableAsync(string isrc, int? excludingTrackId = null, CancellationToken cancellationToken = default) => unitOfWork.Tracks.IsIsrcUniqueAsync(isrc, excludingTrackId, cancellationToken);

    public async Task<TrackDTO> CreateAsync(TrackRequestDTO request, CancellationToken cancellationToken = default)
    {
        var track = MapToEntity(request);
        ValidateTrack(track);
        await EnsureArtistExistsAsync(track.ArtistId, cancellationToken);
        if (!await unitOfWork.Tracks.IsIsrcUniqueAsync(track.ISRC, cancellationToken: cancellationToken))
            throw new ConflictException("A track with this ISRC already exists.");

        await unitOfWork.Tracks.AddAsync(track, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDTO(track);
    }

    public async Task<PaginatedResult<TrackDTO>> GetAllAsync(
        int? artistId,
        string? genre,
        string? status,
        bool statusProvided,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var normalizedGenre = string.IsNullOrWhiteSpace(genre) ? null : genre.Trim();
        var parsedStatus = ParseStatus(status, statusProvided);
        ValidateFilters(artistId, normalizedGenre, parsedStatus, pageIndex, pageSize);

        var specification = new TrackFilterSpecification(artistId, normalizedGenre, parsedStatus);
        var (tracks, totalCount) = await unitOfWork.Tracks.GetPagedAsync(specification, pageIndex, pageSize, cancellationToken);
        return new PaginatedResult<TrackDTO>(pageIndex, pageSize, totalCount, tracks.Select(MapToDTO).ToArray());
    }

    public async Task<TrackDetailsDTO> GetByIdAsync(int trackId, CancellationToken cancellationToken = default)
    {
        var track = await unitOfWork.Tracks.GetAsync(new TrackWithDetailsSpecification(trackId), cancellationToken)
            ?? throw new TrackNotFoundException(trackId);

        var artist = track.Artist;
        var distributions = track.TrackDistributions
            .Select(distribution => new TrackDistributionDTO(
                distribution.Id,
                distribution.DspId,
                distribution.DSP.Name,
                distribution.SubmittedAt,
                distribution.Status.ToString()))
            .ToArray();

        return new TrackDetailsDTO(
            track.Id,
            track.Title,
            track.ISRC,
            track.Genre,
            track.ReleaseDate,
            track.Status.ToString(),
            new ArtistDTO(artist.Id, artist.Name, artist.Email, artist.Country),
            distributions);
    }

    public async Task UpdateAsync(int trackId, TrackRequestDTO request, CancellationToken cancellationToken = default)
    {
        var track = await GetTrackAsync(trackId, cancellationToken);
        var updatedTrack = MapToEntity(request);
        ValidateTrack(updatedTrack);
        await EnsureArtistExistsAsync(updatedTrack.ArtistId, cancellationToken);
        if (!await unitOfWork.Tracks.IsIsrcUniqueAsync(updatedTrack.ISRC, trackId, cancellationToken))
            throw new ConflictException("A track with this ISRC already exists.");

        track.Title = updatedTrack.Title;
        track.ISRC = updatedTrack.ISRC;
        track.Genre = updatedTrack.Genre;
        track.ReleaseDate = updatedTrack.ReleaseDate;
        track.ArtistId = updatedTrack.ArtistId;
        unitOfWork.Tracks.Update(track);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateStatusAsync(int trackId, int status, CancellationToken cancellationToken = default)
    {
        var parsedStatus = (TrackStatus)status;
        if (!Enum.IsDefined(parsedStatus))
            throw new ValidationException(["Status must be 1 (Draft), 2 (Submitted), or 3 (Distributed)."]);

        var track = await GetTrackAsync(trackId, cancellationToken);
        track.Status = parsedStatus;
        unitOfWork.Tracks.Update(track);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int trackId, CancellationToken cancellationToken = default)
    {
        var track = await GetTrackAsync(trackId, cancellationToken);
        if (await unitOfWork.Tracks.HasDistributionsAsync(trackId, cancellationToken))
            throw new ConflictException("A track with distributions cannot be deleted.");

        unitOfWork.Tracks.Delete(track);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Track> GetTrackAsync(int trackId, CancellationToken cancellationToken) =>
        await unitOfWork.Tracks.GetAsync(trackId, cancellationToken)
        ?? throw new TrackNotFoundException(trackId);

    private async Task EnsureArtistExistsAsync(int artistId, CancellationToken cancellationToken)
    {
        if (await unitOfWork.Artists.GetAsync(artistId, cancellationToken) is null)
            throw new ValidationException(["The specified Artist does not exist."]);
    }

    private static Track MapToEntity(TrackRequestDTO request) => new()
    {
        Title = request.Title?.Trim() ?? string.Empty,
        ISRC = request.ISRC?.Trim().ToUpperInvariant() ?? string.Empty,
        Genre = request.Genre?.Trim() ?? string.Empty,
        ReleaseDate = request.ReleaseDate,
        ArtistId = request.ArtistId
    };

    private static TrackDTO MapToDTO(Track track) => new(
        track.Id,
        track.Title,
        track.ISRC,
        track.Genre,
        track.ReleaseDate,
        track.ArtistId,
        track.Status.ToString());

    private static TrackStatus? ParseStatus(string? status, bool statusProvided)
    {
        if (!statusProvided)
            return null;

        var statusName = status?.Trim();
        if (string.IsNullOrWhiteSpace(statusName) ||
            !Enum.GetNames<TrackStatus>().Contains(statusName, StringComparer.OrdinalIgnoreCase))
            throw new ValidationException(["Status must be one of: Draft, Submitted, or Distributed."]);

        return Enum.Parse<TrackStatus>(statusName, ignoreCase: true);
    }

    private static void ValidateTrack(Track track)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(track.Title) || track.Title.Length > 300)
            errors.Add("Title is required and must be 300 characters or fewer.");
        if (!Regex.IsMatch(track.ISRC, "^[A-Z]{2}[A-Z0-9]{3}[0-9]{7}$", RegexOptions.CultureInvariant))
            errors.Add("ISRC must be 12 characters: two letters, three alphanumeric characters, and seven digits.");
        if (string.IsNullOrWhiteSpace(track.Genre) || track.Genre.Length > 100)
            errors.Add("Genre is required and must be 100 characters or fewer.");
        if (track.ReleaseDate == default)
            errors.Add("ReleaseDate is required.");
        if (track.ArtistId <= 0)
            errors.Add("ArtistId must be a positive integer.");

        if (errors.Count > 0)
            throw new ValidationException(errors);
    }

    private static void ValidateFilters(int? artistId, string? genre, TrackStatus? status, int pageIndex, int pageSize)
    {
        var errors = new List<string>();
        if (artistId is <= 0)
            errors.Add("ArtistId must be a positive integer.");
        if (genre is { Length: > 100 })
            errors.Add("Genre must be 100 characters or fewer.");
        if (pageIndex < 1)
            errors.Add("PageIndex must be greater than zero.");
        if (pageSize is < 1 or > 100)
            errors.Add("PageSize must be between 1 and 100.");
        if (pageIndex > 0 && pageSize > 0 && (long)(pageIndex - 1) * pageSize > int.MaxValue)
            errors.Add("The requested page is outside the supported range.");

        if (errors.Count > 0)
            throw new ValidationException(errors);
    }
}
