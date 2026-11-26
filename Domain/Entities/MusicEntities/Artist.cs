namespace Domain.Entities.MusicEntities;

public class Artist : BaseEntity<int>
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Country { get; set; } = null!;
    public ICollection<Track> Tracks { get; set; } = new List<Track>();
}
