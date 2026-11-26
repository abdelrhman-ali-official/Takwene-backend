using Domain.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using System.Linq.Expressions;

namespace Persistence.Repositories;

public class GenericRepository<TEntity, TKey>(StoreContext storeContext) : IGenericRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
{
    private readonly DbSet<TEntity> _dbSet = storeContext.Set<TEntity>();

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        _dbSet.AddAsync(entity, cancellationToken).AsTask();

    public void Delete(TEntity entity) => _dbSet.Remove(entity);

    public void Update(TEntity entity) => _dbSet.Update(entity);

    public Task<TEntity?> GetAsync(TKey id, CancellationToken cancellationToken = default) =>
        _dbSet.AsNoTracking().FirstOrDefaultAsync(entity => entity.Id!.Equals(id), cancellationToken);

    public Task<TEntity?> GetAsync(Specifications<TEntity> specifications, CancellationToken cancellationToken = default) =>
        ApplySpecifications(specifications, applyPagination: true).FirstOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbSet.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<TEntity>> GetAllAsync(Specifications<TEntity> specifications, CancellationToken cancellationToken = default) =>
        await ApplySpecifications(specifications, applyPagination: true).ToListAsync(cancellationToken);

    public Task<int> CountAsync(Specifications<TEntity> specifications, CancellationToken cancellationToken = default) =>
        ApplySpecifications(specifications, applyPagination: false).CountAsync(cancellationToken);

    public async Task<(IEnumerable<TEntity> Entities, int TotalCount)> GetPagedAsync(
        Specifications<TEntity> specifications,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (pageIndex < 1) throw new ArgumentOutOfRangeException(nameof(pageIndex));
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));

        var query = ApplySpecifications(specifications, applyPagination: false);
        var totalCount = await query.CountAsync(cancellationToken);
        var entities = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return (entities, totalCount);
    }

    public async Task<IEnumerable<TKey>> GetAllIdsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
        await _dbSet.AsNoTracking().Where(predicate).Select(entity => entity.Id).ToListAsync(cancellationToken);

    private IQueryable<TEntity> ApplySpecifications(Specifications<TEntity> specifications, bool applyPagination) =>
        SpecificationEvaluator.GetQuery(_dbSet.AsNoTracking(), specifications, applyPagination);
}
