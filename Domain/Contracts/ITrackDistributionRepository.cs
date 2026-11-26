using Domain.Entities.MusicEntities;
namespace Domain.Contracts;
public interface ITrackDistributionRepository : IGenericRepository<TrackDistribution, int>
{
    Task<bool> ExistsAsync(int trackId, int dspId, CancellationToken cancellationToken = default);
}
