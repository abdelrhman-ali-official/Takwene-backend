using Domain.Contracts;
using Domain.Entities.MusicEntities;

namespace Services.Specifications;

public sealed class TrackWithDetailsSpecification : Specifications<Track>
{
    public TrackWithDetailsSpecification(int trackId)
        : base(track => track.Id == trackId)
    {
        AddInclude(track => track.Artist);
        AddInclude("TrackDistributions.DSP");
    }
}
