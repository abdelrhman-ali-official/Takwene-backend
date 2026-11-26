using Domain.Contracts;
using Domain.Entities.MusicEntities;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
namespace Persistence.Repositories;
public class TrackDistributionRepository : GenericRepository<TrackDistribution, int>, ITrackDistributionRepository
{
    private readonly StoreContext _context;
    public TrackDistributionRepository(StoreContext context) : base(context) => _context = context;
    public Task<bool> ExistsAsync(int trackId, int dspId, CancellationToken cancellationToken = default) => _context.TrackDistributions.AnyAsync(x => x.TrackId == trackId && x.DspId == dspId, cancellationToken);
}
