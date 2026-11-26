using System.Collections.Concurrent;
using Domain.Contracts;
using Domain.Entities;
using Persistence.Data;

namespace Persistence.Repositories;

public class UnitOfWork : IUnitOFWork
{
    private readonly StoreContext _storeContext;
    private readonly ConcurrentDictionary<string, object> _repositories = new();

    public UnitOfWork(StoreContext storeContext)
    {
        _storeContext = storeContext;
        Artists = new ArtistRepository(storeContext);
        Tracks = new TrackRepository(storeContext);
        DSPs = new DspRepository(storeContext);
        TrackDistributions = new TrackDistributionRepository(storeContext);
    }

    public IArtistRepository Artists { get; }
    public ITrackRepository Tracks { get; }
    public IDspRepository DSPs { get; }
    public ITrackDistributionRepository TrackDistributions { get; }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => _storeContext.SaveChangesAsync(cancellationToken);
    public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey> =>
        (IGenericRepository<TEntity, TKey>)_repositories.GetOrAdd(typeof(TEntity).Name, _ => new GenericRepository<TEntity, TKey>(_storeContext));
}
