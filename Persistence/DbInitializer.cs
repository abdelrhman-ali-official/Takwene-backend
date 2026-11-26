using Domain.Contracts;
using Domain.Entities.MusicEntities;
using Domain.Entities.SecurityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Data;

namespace Persistence;

public class DbInitializer(
    StoreContext storeContext,
    RoleManager<IdentityRole> roleManager,
    ILogger<DbInitializer> logger) : IDbInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Applying pending database migrations and initializing Takwene reference data.");
        await storeContext.Database.MigrateAsync(cancellationToken);
        await EnsureRolesAsync(cancellationToken);
        await SeedMusicDataAsync(cancellationToken);
    }

    public async Task VerifyInitializedAsync(CancellationToken cancellationToken = default)
    {
        var pendingMigrations = await storeContext.Database.GetPendingMigrationsAsync(cancellationToken);
        if (pendingMigrations.Any())
            throw new InvalidOperationException("Database has pending migrations. Apply migrations through the approved deployment process before starting the API.");

        foreach (var role in Enum.GetValues<Role>())
        {
            if (!await roleManager.RoleExistsAsync(role.ToString()))
                throw new InvalidOperationException($"Required Identity role '{role}' is missing. Initialize the database through the approved deployment process.");
        }
    }

    private async Task EnsureRolesAsync(CancellationToken cancellationToken)
    {
        foreach (var role in Enum.GetValues<Role>())
        {
            cancellationToken.ThrowIfCancellationRequested();
            var name = role.ToString();
            if (await roleManager.RoleExistsAsync(name))
                continue;

            var result = await roleManager.CreateAsync(new IdentityRole(name));
            if (!result.Succeeded)
                throw new InvalidOperationException($"Unable to create Identity role '{name}': {string.Join("; ", result.Errors.Select(error => error.Description))}");
        }
    }

    private async Task SeedMusicDataAsync(CancellationToken cancellationToken)
    {
        var artists = new[]
        {
            new Artist { Name = "Noura Vale", Email = "noura.vale@example.test", Country = "Egypt" },
            new Artist { Name = "Jonah Kade", Email = "jonah.kade@example.test", Country = "Canada" },
            new Artist { Name = "Mira Sol", Email = "mira.sol@example.test", Country = "Spain" }
        };

        var existingArtistEmails = await storeContext.Artists.Select(artist => artist.Email).ToListAsync(cancellationToken);
        var newArtists = artists.Where(artist => !existingArtistEmails.Contains(artist.Email, StringComparer.OrdinalIgnoreCase));
        await storeContext.Artists.AddRangeAsync(newArtists, cancellationToken);

        var dsps = new[]
        {
            new DSP { Name = "Spotify" },
            new DSP { Name = "Apple Music" },
            new DSP { Name = "YouTube Music" }
        };
        var existingDspNames = await storeContext.DSPs.Select(dsp => dsp.Name).ToListAsync(cancellationToken);
        var newDsps = dsps.Where(dsp => !existingDspNames.Contains(dsp.Name, StringComparer.OrdinalIgnoreCase));
        await storeContext.DSPs.AddRangeAsync(newDsps, cancellationToken);
        await storeContext.SaveChangesAsync(cancellationToken);

        var artistIds = await storeContext.Artists.ToDictionaryAsync(artist => artist.Email, artist => artist.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);
        var tracks = new[]
        {
            new Track { Title = "Cairo After Rain", ArtistId = artistIds["noura.vale@example.test"], ISRC = "EGNVA2400001", ReleaseDate = new DateOnly(2024, 2, 9), Genre = "Indie Pop", Status = TrackStatus.Distributed },
            new Track { Title = "Lanterns", ArtistId = artistIds["noura.vale@example.test"], ISRC = "EGNVA2400002", ReleaseDate = new DateOnly(2024, 4, 19), Genre = "Electronic", Status = TrackStatus.Submitted },
            new Track { Title = "Blue Transit", ArtistId = artistIds["jonah.kade@example.test"], ISRC = "CAJKA2400001", ReleaseDate = new DateOnly(2024, 1, 26), Genre = "Alternative Rock", Status = TrackStatus.Distributed },
            new Track { Title = "Northbound", ArtistId = artistIds["jonah.kade@example.test"], ISRC = "CAJKA2400002", ReleaseDate = new DateOnly(2024, 6, 7), Genre = "Folk", Status = TrackStatus.Draft },
            new Track { Title = "Sol y Sal", ArtistId = artistIds["mira.sol@example.test"], ISRC = "ESMSL2400001", ReleaseDate = new DateOnly(2024, 3, 15), Genre = "Latin Pop", Status = TrackStatus.Distributed },
            new Track { Title = "Mercury Steps", ArtistId = artistIds["mira.sol@example.test"], ISRC = "ESMSL2400002", ReleaseDate = new DateOnly(2024, 5, 3), Genre = "Dance", Status = TrackStatus.Submitted },
            new Track { Title = "Quiet Current", ArtistId = artistIds["noura.vale@example.test"], ISRC = "EGNVA2400003", ReleaseDate = new DateOnly(2024, 7, 12), Genre = "Ambient", Status = TrackStatus.Draft },
            new Track { Title = "Silver Mile", ArtistId = artistIds["jonah.kade@example.test"], ISRC = "CAJKA2400003", ReleaseDate = new DateOnly(2024, 8, 23), Genre = "Singer-Songwriter", Status = TrackStatus.Submitted }
        };

        var existingIsrcs = await storeContext.Tracks.Select(track => track.ISRC).ToListAsync(cancellationToken);
        await storeContext.Tracks.AddRangeAsync(tracks.Where(track => !existingIsrcs.Contains(track.ISRC, StringComparer.OrdinalIgnoreCase)), cancellationToken);
        await storeContext.SaveChangesAsync(cancellationToken);

        var trackIds = await storeContext.Tracks.ToDictionaryAsync(track => track.ISRC, track => track.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);
        var dspIds = await storeContext.DSPs.ToDictionaryAsync(dsp => dsp.Name, dsp => dsp.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);
        var distributions = new[]
        {
            new TrackDistribution { TrackId = trackIds["EGNVA2400001"], DspId = dspIds["Spotify"], SubmittedAt = DateTime.UtcNow.AddDays(-30), Status = DistributionStatus.Live },
            new TrackDistribution { TrackId = trackIds["EGNVA2400001"], DspId = dspIds["Apple Music"], SubmittedAt = DateTime.UtcNow.AddDays(-30), Status = DistributionStatus.Live },
            new TrackDistribution { TrackId = trackIds["CAJKA2400001"], DspId = dspIds["YouTube Music"], SubmittedAt = DateTime.UtcNow.AddDays(-24), Status = DistributionStatus.Live },
            new TrackDistribution { TrackId = trackIds["ESMSL2400002"], DspId = dspIds["Spotify"], SubmittedAt = DateTime.UtcNow.AddDays(-5), Status = DistributionStatus.Pending }
        };

        var existingDistributions = await storeContext.TrackDistributions
            .Select(distribution => new { distribution.TrackId, distribution.DspId })
            .ToListAsync(cancellationToken);
        await storeContext.TrackDistributions.AddRangeAsync(
            distributions.Where(distribution => !existingDistributions.Any(existing => existing.TrackId == distribution.TrackId && existing.DspId == distribution.DspId)),
            cancellationToken);
        await storeContext.SaveChangesAsync(cancellationToken);
    }
}
