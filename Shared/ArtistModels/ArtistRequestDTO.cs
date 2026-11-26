using System.ComponentModel.DataAnnotations;

namespace Shared.ArtistModels;

public sealed record ArtistRequestDTO
{
    [Required]
    [StringLength(200)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Country { get; init; } = string.Empty;
}