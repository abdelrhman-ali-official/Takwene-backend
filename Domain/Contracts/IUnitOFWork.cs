using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IUnitOFWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        IArtistRepository Artists { get; }
        ITrackRepository Tracks { get; }
        IDspRepository DSPs { get; }
        ITrackDistributionRepository TrackDistributions { get; }

        IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>;
    }
}
