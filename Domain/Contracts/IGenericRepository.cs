using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        Task<TEntity?> GetAsync(TKey id, CancellationToken cancellationToken = default);
        Task<TEntity?> GetAsync(Specifications<TEntity> specifications, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Specifications<TEntity> specifications, CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> GetAllAsync(Specifications<TEntity> specifications, CancellationToken cancellationToken = default);

        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        void Delete(TEntity entity);
        void Update(TEntity entity);
        Task<(IEnumerable<TEntity> Entities, int TotalCount)> GetPagedAsync(Specifications<TEntity> specifications, int pageIndex, int pageSize, CancellationToken cancellationToken = default);
        
        Task<IEnumerable<TKey>> GetAllIdsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
