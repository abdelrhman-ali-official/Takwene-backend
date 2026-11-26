using Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Data;
using Persistence.Repositories;
using Services;
using Services.Abstractions;
using Shared.SecurityModels;
using System.Text;

namespace Takwene.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("JwtOptions");
        var jwtOptions = jwtSection.Get<JwtOptions>() ?? throw new InvalidOperationException("JwtOptions configuration is required.");
        if (string.IsNullOrWhiteSpace(jwtOptions.SecretKey) || Encoding.UTF8.GetByteCount(jwtOptions.SecretKey) < 32)
            throw new InvalidOperationException("Configure JwtOptions:SecretKey with a secret of at least 32 bytes through User Secrets or an environment variable.");
        if (string.IsNullOrWhiteSpace(jwtOptions.Issuer) || string.IsNullOrWhiteSpace(jwtOptions.Audience) || jwtOptions.DurationInDays is <= 0 or > 7)
            throw new InvalidOperationException("JwtOptions Issuer, Audience, and a DurationInDays value between 1 and 7 must be configured.");

        services.AddControllers().AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly);
        services.AddTakweneSwagger();
        services.Configure<JwtOptions>(jwtSection);
        services.AddDbContext<StoreContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultSQLConnection")));
        services.AddTakweneIdentity();
        services.AddTakweneJwtAuthentication(jwtOptions);
        services.AddAuthorization();
        services.AddScoped<IUnitOFWork, UnitOfWork>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IArtistService, ArtistService>();
        services.AddScoped<ITrackService, TrackService>();
        services.AddScoped<IDSPService, DSPService>();
        services.AddScoped<ITrackDistributionService, TrackDistributionService>();
        services.AddScoped<IDbInitializer, DbInitializer>();

        return services;
    }
}