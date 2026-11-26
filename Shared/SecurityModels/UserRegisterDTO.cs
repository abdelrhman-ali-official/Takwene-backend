using System.ComponentModel.DataAnnotations;

namespace Shared.SecurityModels;

public record UserRegisterDTO
{
    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(100)]
    public string FirstName { get; init; } = null!;

    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(100)]
    public string LastName { get; init; } = null!;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; init; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; init; } = null!;

    [Required(ErrorMessage = "Username is required.")]
    [MaxLength(256)]
    public string UserName { get; init; } = null!;

    [Phone]
    public string? PhoneNumber { get; init; }

    public int? Gender { get; init; }
}
