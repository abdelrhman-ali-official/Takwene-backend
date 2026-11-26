using Domain.Entities.MusicEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Persistence.Data.Configurations;
public class TrackDistributionConfiguration : IEntityTypeConfiguration<TrackDistribution>
{
    public void Configure(EntityTypeBuilder<TrackDistribution> builder)
    {
        builder.ToTable("TrackDistributions"); builder.HasKey(x => x.Id);
        builder.Property(x => x.SubmittedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSUTCDATETIME()").IsRequired();
        builder.Property(x => x.Status).HasConversion<byte>().IsRequired();
        builder.ToTable(table => table.HasCheckConstraint("CK_TrackDistributions_Status", "[Status] IN (1, 2, 3)"));
        builder.HasIndex(x => new { x.TrackId, x.DspId }).IsUnique();
        builder.HasOne(x => x.Track).WithMany(x => x.TrackDistributions).HasForeignKey(x => x.TrackId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.DSP).WithMany(x => x.TrackDistributions).HasForeignKey(x => x.DspId).OnDelete(DeleteBehavior.Restrict);
    }
}
