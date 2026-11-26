using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Shared.SecurityModels;
using System.Security.Claims;
using System.Text;

namespace Takwene.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddTakweneJwtAuthentication(this IServiceCollection services, JwtOptions jwtOptions)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                NameClaimType = ClaimTypes.Email,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.FromMinutes(1)
            });

        return services;
    }
}