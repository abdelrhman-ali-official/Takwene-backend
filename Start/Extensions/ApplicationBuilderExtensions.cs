using Domain.Contracts;

namespace Takwene.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseBootstrap");

        try
        {
            if (app.Environment.IsProduction())
            {
                logger.LogInformation("Verifying database initialization; production migrations are managed by deployment.");
                await initializer.VerifyInitializedAsync();
            }
            else
            {
                await initializer.InitializeAsync();
            }
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, "Database bootstrap failed. The API will not start.");
            throw;
        }
    }
}