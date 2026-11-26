using Domain.Contracts;
using Domain.Entities.MusicEntities;
using Domain.Exceptions;
using Services.Abstractions;
using Shared.ArtistModels;
namespace Services;
public class ArtistService(IUnitOFWork unitOfWork) : IArtistService
{
    public async Task<bool> ExistsAsync(int artistId, CancellationToken cancellationToken = default) => await unitOfWork.Artists.GetAsync(artistId, cancellationToken) is not null;

    public async Task<ArtistDTO> CreateAsync(ArtistRequestDTO request, CancellationToken cancellationToken = default)
    {
        var artist = MapToEntity(request);
        Validate(artist);
        if (await unitOfWork.Artists.EmailExistsAsync(artist.Email, cancellationToken: cancellationToken))
            throw new ConflictException("An artist with this email already exists.");

        await unitOfWork.Artists.AddAsync(artist, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDTO(artist);
    }

    public async Task<IReadOnlyList<ArtistDTO>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var artists = await unitOfWork.Artists.GetAllAsync(cancellationToken);
        return artists.Select(MapToDTO).ToArray();
    }

    public async Task<ArtistDTO> GetByIdAsync(int artistId, CancellationToken cancellationToken = default)
    {
        var artist = await GetArtistAsync(artistId, cancellationToken);
        return MapToDTO(artist);
    }

    public async Task UpdateAsync(int artistId, ArtistRequestDTO request, CancellationToken cancellationToken = default)
    {
        var artist = await GetArtistAsync(artistId, cancellationToken);
        var updatedArtist = MapToEntity(request);
        Validate(updatedArtist);
        if (await unitOfWork.Artists.EmailExistsAsync(updatedArtist.Email, artistId, cancellationToken))
            throw new ConflictException("An artist with this email already exists.");

        artist.Name = updatedArtist.Name;
        artist.Email = updatedArtist.Email;
        artist.Country = updatedArtist.Country;
        unitOfWork.Artists.Update(artist);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int artistId, CancellationToken cancellationToken = default)
    {
        var artist = await GetArtistAsync(artistId, cancellationToken);
        if (await unitOfWork.Artists.HasTracksAsync(artistId, cancellationToken))
            throw new ConflictException("An artist with tracks cannot be deleted.");

        unitOfWork.Artists.Delete(artist);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Artist> GetArtistAsync(int artistId, CancellationToken cancellationToken) =>
        await unitOfWork.Artists.GetAsync(artistId, cancellationToken)
        ?? throw new ArtistNotFoundException(artistId);

    private static Artist MapToEntity(ArtistRequestDTO request) => new()
    {
        Name = request.Name?.Trim() ?? string.Empty,
        Email = request.Email?.Trim() ?? string.Empty,
        Country = request.Country?.Trim() ?? string.Empty
    };

    private static ArtistDTO MapToDTO(Artist artist) => new(artist.Id, artist.Name, artist.Email, artist.Country);

    private static void Validate(Artist artist)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(artist.Name) || artist.Name.Length > 200)
            errors.Add("Name is required and must be 200 characters or fewer.");
        if (string.IsNullOrWhiteSpace(artist.Email) || artist.Email.Length > 256 || !System.Net.Mail.MailAddress.TryCreate(artist.Email, out var address) || address.Address != artist.Email)
            errors.Add("A valid email of 256 characters or fewer is required.");
        if (string.IsNullOrWhiteSpace(artist.Country) || artist.Country.Length > 100)
            errors.Add("Country is required and must be 100 characters or fewer.");

        if (errors.Count > 0)
            throw new ValidationException(errors);
    }
}
