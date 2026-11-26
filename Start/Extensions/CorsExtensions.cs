namespace Takwene.Api.Extensions;

public static class CorsExtensions
{
    public const string PolicyName = "TakweneAngularPolicy";

    public static IServiceCollection AddTakweneCors(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var configuredOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        string[] allowedOrigins;

        if (environment.IsDevelopment())
        {
            allowedOrigins = configuredOrigins.Length > 0
                ? configuredOrigins
                : ["http://localhost:4200"];
        }
        else
        {
            if (configuredOrigins.Length == 0)
                throw new InvalidOperationException("Configure Cors:AllowedOrigins with approved Angular origins for non-development environments.");

            allowedOrigins = configuredOrigins;
        }

        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy => policy
                .WithOrigins(allowedOrigins)
                .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
                .AllowAnyHeader());
        });

        return services;
    }
}
