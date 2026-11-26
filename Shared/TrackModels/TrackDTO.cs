namespace Shared.TrackModels;

public sealed record TrackDTO(
    int Id,
    string Title,
    string ISRC,
    string Genre,
    DateOnly ReleaseDate,
    int ArtistId,
    string Status);