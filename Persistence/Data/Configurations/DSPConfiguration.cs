using Domain.Entities.MusicEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Persistence.Data.Configurations;
public class DSPConfiguration : IEntityTypeConfiguration<DSP>
{
    public void Configure(EntityTypeBuilder<DSP> builder)
    {
        builder.ToTable("DSPs"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.HasIndex(x => x.Name).IsUnique();
    }
}
