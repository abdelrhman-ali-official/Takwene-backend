using Domain.Contracts;
using Domain.Entities.MusicEntities;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
namespace Persistence.Repositories;
public class TrackRepository : GenericRepository<Track, int>, ITrackRepository
{
    private readonly StoreContext _context;
    public TrackRepository(StoreContext context) : base(context) => _context = context;
    public async Task<bool> IsIsrcUniqueAsync(string isrc, int? excludingTrackId = null, CancellationToken cancellationToken = default) =>
        !await _context.Tracks.AnyAsync(x => x.ISRC == isrc && (!excludingTrackId.HasValue || x.Id != excludingTrackId.Value), cancellationToken);

    public Task<bool> HasDistributionsAsync(int trackId, CancellationToken cancellationToken = default) =>
        _context.TrackDistributions.AnyAsync(distribution => distribution.TrackId == trackId, cancellationToken);
}
