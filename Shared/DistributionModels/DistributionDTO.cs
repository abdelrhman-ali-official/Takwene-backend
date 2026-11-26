namespace Shared.DistributionModels;

public sealed record DistributionDTO(
    int Id,
    int TrackId,
    int DspId,
    string DspName,
    DateTime SubmittedAt,
    string Status);