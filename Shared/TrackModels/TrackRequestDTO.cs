using System.ComponentModel.DataAnnotations;

namespace Shared.TrackModels;

public sealed record TrackRequestDTO
{
    [Required]
    [StringLength(300)]
    public string Title { get; init; } = string.Empty;

    [Required]
    [StringLength(12, MinimumLength = 12)]
    public string ISRC { get; init; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Genre { get; init; } = string.Empty;

    public DateOnly ReleaseDate { get; init; }

    [Range(1, int.MaxValue)]
    public int ArtistId { get; init; }
}