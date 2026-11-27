using System.ComponentModel.DataAnnotations;

namespace Shared.TrackModels;

public sealed record TrackStatusRequestDTO
{
    [Range(1, 3, ErrorMessage = "Status must be 1 (Draft), 2 (Submitted), or 3 (Distributed).")]
    public int Status { get; init; }
}