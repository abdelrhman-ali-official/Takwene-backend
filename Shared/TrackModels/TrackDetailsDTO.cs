using Shared.ArtistModels;

namespace Shared.TrackModels;

public sealed record TrackDetailsDTO(
    int Id,
    string Title,
    string ISRC,
    string Genre,
    DateOnly ReleaseDate,
    string Status,
    ArtistDTO Artist,
    IReadOnlyList<TrackDistributionDTO> Distributions);

public sealed record TrackDistributionDTO(
    int Id,
    int DspId,
    string DspName,
    DateTime SubmittedAt,
    string Status);