using System.ComponentModel.DataAnnotations;

namespace Shared.DistributionModels;

public sealed record DistributeTrackRequestDTO
{
    [Required]
    [MinLength(1, ErrorMessage = "At least one DSP ID is required.")]
    public IReadOnlyList<int> DspIds { get; init; } = Array.Empty<int>();
}
