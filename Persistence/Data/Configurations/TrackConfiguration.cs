using Domain.Entities.MusicEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Persistence.Data.Configurations;
public class TrackConfiguration : IEntityTypeConfiguration<Track>
{
    public void Configure(EntityTypeBuilder<Track> builder)
    {
        builder.ToTable("Tracks"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(300);
        builder.Property(x => x.ISRC).IsRequired().HasMaxLength(12);
        builder.Property(x => x.ReleaseDate).HasColumnType("date");
        builder.Property(x => x.Genre).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Status).HasConversion<byte>().IsRequired();
        builder.ToTable(table => table.HasCheckConstraint("CK_Tracks_Status", "[Status] IN (1, 2, 3)"));
        builder.ToTable(table => table.HasCheckConstraint("CK_Tracks_ISRC", "LEN([ISRC]) = 12 AND [ISRC] NOT LIKE '%[^A-Z0-9]%' AND SUBSTRING([ISRC], 1, 2) NOT LIKE '%[^A-Z]%' AND SUBSTRING([ISRC], 6, 7) NOT LIKE '%[^0-9]%'"));
        builder.HasIndex(x => x.ISRC).IsUnique();
        builder.HasOne(x => x.Artist).WithMany(x => x.Tracks).HasForeignKey(x => x.ArtistId).OnDelete(DeleteBehavior.Restrict);
    }
}
