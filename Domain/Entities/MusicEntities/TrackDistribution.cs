namespace Domain.Entities.MusicEntities;

public class TrackDistribution : BaseEntity<int>
{
    public int TrackId { get; set; }
    public int DspId { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DistributionStatus Status { get; set; } = DistributionStatus.Pending;
    public Track Track { get; set; } = null!;
    public DSP DSP { get; set; } = null!;
}

public enum DistributionStatus : byte { Pending = 1, Live = 2, Rejected = 3 }
