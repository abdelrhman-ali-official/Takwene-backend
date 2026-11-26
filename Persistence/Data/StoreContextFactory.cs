using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Persistence.Data;

public class StoreContextFactory : IDesignTimeDbContextFactory<StoreContext>
{
    public StoreContext CreateDbContext(string[] args)
    {
        var startupProjectPath = FindStartupProjectPath();
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var configuration = new ConfigurationBuilder()
            .SetBasePath(startupProjectPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddUserSecrets<StoreContextFactory>(optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultSQLConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("ConnectionStrings:DefaultSQLConnection is required to create StoreContext at design time.");

        return new StoreContext(new DbContextOptionsBuilder<StoreContext>()
            .UseSqlServer(connectionString)
            .Options);
    }

    private static string FindStartupProjectPath()
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (directory is not null)
        {
            var startProject = Path.Combine(directory.FullName, "Start", "Start.csproj");
            if (File.Exists(startProject))
                return Path.GetDirectoryName(startProject)!;

            if (File.Exists(Path.Combine(directory.FullName, "Start.csproj")))
                return directory.FullName;

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not locate the Start project required for design-time configuration.");
    }
}
