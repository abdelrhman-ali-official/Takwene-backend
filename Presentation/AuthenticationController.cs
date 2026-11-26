using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.SecurityModels;
namespace Presentation;
public class AuthenticationController(IAuthenticationService authenticationService) : ApiController
{
    [AllowAnonymous, HttpPost("register")]
    public async Task<ActionResult<UserResultDTO>> Register(UserRegisterDTO registerDto, CancellationToken cancellationToken) => Ok(await authenticationService.RegisterAsync(registerDto, cancellationToken));
    [AllowAnonymous, HttpPost("login")]
    public async Task<ActionResult<UserResultDTO>> Login(LoginDTO loginDto, CancellationToken cancellationToken) => Ok(await authenticationService.LoginAsync(loginDto, cancellationToken));
}
