using Domain.Contracts;
using Domain.Entities.MusicEntities;

namespace Services.Specifications;

public sealed class DistributionByIdSpecification : Specifications<TrackDistribution>
{
    public DistributionByIdSpecification(int distributionId)
        : base(distribution => distribution.Id == distributionId)
    {
        AddInclude(distribution => distribution.DSP);
    }
}