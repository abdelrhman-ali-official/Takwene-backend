using System.ComponentModel.DataAnnotations;

namespace Shared.DistributionModels;

public sealed record DistributionRequestDTO
{
    [Range(1, int.MaxValue)]
    public int DspId { get; init; }
}