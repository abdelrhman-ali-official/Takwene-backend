using System.ComponentModel.DataAnnotations;

namespace Shared.TrackModels;

public sealed record TrackStatusRequestDTO
{
    [Required]
    public string Status { get; init; } = string.Empty;
}