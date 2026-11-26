namespace Domain.Entities.MusicEntities;

public class Track : BaseEntity<int>
{
    public string Title { get; set; } = null!;
    public int ArtistId { get; set; }
    public string ISRC { get; set; } = null!;
    public DateOnly ReleaseDate { get; set; }
    public string Genre { get; set; } = null!;
    public TrackStatus Status { get; set; } = TrackStatus.Draft;
    public Artist Artist { get; set; } = null!;
    public ICollection<TrackDistribution> TrackDistributions { get; set; } = new List<TrackDistribution>();
}

public enum TrackStatus : byte { Draft = 1, Submitted = 2, Distributed = 3 }
