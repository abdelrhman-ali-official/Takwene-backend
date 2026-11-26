using Domain.Entities.MusicEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Persistence.Data.Configurations;
public class ArtistConfiguration : IEntityTypeConfiguration<Artist>
{
    public void Configure(EntityTypeBuilder<Artist> builder)
    {
        builder.ToTable("Artists"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(256);
        builder.Property(x => x.Country).IsRequired().HasMaxLength(100);
        builder.HasIndex(x => x.Email).IsUnique();
        builder.ToTable(table => table.HasCheckConstraint("CK_Artists_Email", "[Email] LIKE '%_@_%._%'"));
    }
}
