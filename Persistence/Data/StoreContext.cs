using Domain.Entities.MusicEntities;
using Domain.Entities.SecurityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Persistence.Data;

public class StoreContext(DbContextOptions<StoreContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Artist> Artists => Set<Artist>();
    public DbSet<Track> Tracks => Set<Track>();
    public DbSet<DSP> DSPs => Set<DSP>();
    public DbSet<TrackDistribution> TrackDistributions => Set<TrackDistribution>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().Property(user => user.UserRole)
            .HasConversion<byte>()
            .HasDefaultValue(Role.Editor)
            .HasSentinel((Role)0);
        modelBuilder.Entity<User>().Property(user => user.Gender).HasConversion<byte>().HasDefaultValue(Gender.Unspecified);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
