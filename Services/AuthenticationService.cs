using Domain.Entities.SecurityEntities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services.Abstractions;
using Shared.SecurityModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services;

public class AuthenticationService(
    UserManager<User> userManager,
    IOptions<JwtOptions> jwtOptions,
    ILogger<AuthenticationService> logger) : IAuthenticationService
{
    public async Task<UserResultDTO> RegisterAsync(UserRegisterDTO registerModel, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var gender = ParseGender(registerModel.Gender);

        var user = new User
        {
            FirstName = registerModel.FirstName.Trim(),
            LastName = registerModel.LastName.Trim(),
            DisplayName = $"{registerModel.FirstName} {registerModel.LastName}".Trim(),
            Email = registerModel.Email.Trim(),
            UserName = registerModel.UserName.Trim(),
            PhoneNumber = registerModel.PhoneNumber,
            Gender = gender,
            UserRole = Role.Editor
        };

        var createResult = await userManager.CreateAsync(user, registerModel.Password);
        if (!createResult.Succeeded)
            throw new ValidationException(["Unable to create an account with the supplied information."]);

        cancellationToken.ThrowIfCancellationRequested();
        var roleResult = await userManager.AddToRoleAsync(user, Role.Editor.ToString());
        if (!roleResult.Succeeded)
        {
            var deleteResult = await userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
                logger.LogCritical("Failed to compensate for incomplete registration of user {UserId}.", user.Id);

            logger.LogError("Default role assignment failed for user {UserId}.", user.Id);
            throw new InvalidOperationException("Registration could not be completed.");
        }

        return new UserResultDTO(user.DisplayName, user.Email!, await CreateTokenAsync(user, cancellationToken));
    }

    public async Task<UserResultDTO> LoginAsync(LoginDTO loginModel, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await userManager.FindByEmailAsync(loginModel.Email);
        if (user is null || await userManager.IsLockedOutAsync(user))
            throw new UnAuthorizedException("Invalid email or password.");

        if (!await userManager.CheckPasswordAsync(user, loginModel.Password))
        {
            await userManager.AccessFailedAsync(user);
            throw new UnAuthorizedException("Invalid email or password.");
        }

        await userManager.ResetAccessFailedCountAsync(user);
        return new UserResultDTO(user.DisplayName, user.Email!, await CreateTokenAsync(user, cancellationToken));
    }

    private async Task<string> CreateTokenAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var settings = jwtOptions.Value;
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.DisplayName)
        };
        claims.AddRange((await userManager.GetRolesAsync(user)).Select(role => new Claim(ClaimTypes.Role, role)));

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            settings.Issuer,
            settings.Audience,
            claims,
            expires: DateTime.UtcNow.AddDays(settings.DurationInDays),
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static Gender ParseGender(int? gender)
    {
        if (!gender.HasValue)
            return Gender.Unspecified;

        var parsedGender = (Gender)gender.Value;
        if (!Enum.IsDefined(parsedGender))
            throw new ValidationException(["Gender must be 0 (Unspecified), 1 (Male), or 2 (Female)."]);

        return parsedGender;
    }
}
