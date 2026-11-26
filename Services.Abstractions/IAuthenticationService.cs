using Shared.SecurityModels;

namespace Services.Abstractions;

public interface IAuthenticationService
{
    Task<UserResultDTO> RegisterAsync(UserRegisterDTO registerModel, CancellationToken cancellationToken = default);
    Task<UserResultDTO> LoginAsync(LoginDTO loginModel, CancellationToken cancellationToken = default);
}
