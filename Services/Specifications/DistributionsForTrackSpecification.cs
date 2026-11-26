using Domain.Contracts;
using Domain.Entities.MusicEntities;

namespace Services.Specifications;

public sealed class DistributionsForTrackSpecification : Specifications<TrackDistribution>
{
    public DistributionsForTrackSpecification(int trackId)
        : base(distribution => distribution.TrackId == trackId)
    {
        AddInclude(distribution => distribution.DSP);
        setOrderBy(distribution => distribution.Id);
    }
}