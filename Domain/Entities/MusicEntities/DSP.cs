namespace Domain.Entities.MusicEntities;

public class DSP : BaseEntity<int>
{
    public string Name { get; set; } = null!;
    public ICollection<TrackDistribution> TrackDistributions { get; set; } = new List<TrackDistribution>();
}
