using InventoryWebApi.Application.Interfaces;
using InventoryWebApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryWebApi.Infrastructure.Repositories;

/// <summary>
/// EF Core based repository with optional pagination.
/// Pagination is centralized here to avoid repeating Skip/Take logic in controllers.
/// </summary>
public class EfRepository<T> : IRepository<T> where T : class
{
    protected const int DefaultPageSize = 50;
    protected const int MaxPageSize = 200;

    protected readonly ApplicationDbContext DbContext;

    public EfRepository(ApplicationDbContext dbContext)
    {
        DbContext = dbContext;
    }

    protected virtual IQueryable<T> GetAllQuery()
        => DbContext.Set<T>().AsNoTracking();

    protected virtual IQueryable<T> GetByIdQuery()
        => DbContext.Set<T>().AsNoTracking();

    protected virtual IQueryable<T> ApplyGetAllIncludes(IQueryable<T> query) => query;

    protected virtual IQueryable<T> ApplyGetByIdIncludes(IQueryable<T> query) => query;

    public virtual Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var query = ApplyGetByIdIncludes(GetByIdQuery());
        return query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id, cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken cancellationToken = default)
    {
        var query = ApplyGetAllIncludes(GetAllQuery())
            .OrderBy(e => EF.Property<int>(e, "Id"));

        if (pageNumber.HasValue || pageSize.HasValue)
        {
            var normalizedPageNumber = pageNumber.GetValueOrDefault(1);
            if (normalizedPageNumber < 1)
            {
                normalizedPageNumber = 1;
            }

            var normalizedPageSize = pageSize.GetValueOrDefault(DefaultPageSize);
            if (normalizedPageSize < 1)
            {
                normalizedPageSize = DefaultPageSize;
            }

            if (normalizedPageSize > MaxPageSize)
            {
                normalizedPageSize = MaxPageSize;
            }

            var skip = (normalizedPageNumber - 1) * normalizedPageSize;
            query = query.Skip(skip).Take(normalizedPageSize).OrderBy(e => EF.Property<int>(e, "Id"));
        }

        return await query.ToListAsync(cancellationToken);
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbContext.Set<T>().Add(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        // Mark only the root entity as modified.
        DbContext.Entry(entity).State = EntityState.Modified;
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await DbContext.Set<T>().FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id, cancellationToken);
        if (existing is null)
        {
            return;
        }

        DbContext.Set<T>().Remove(existing);
        await DbContext.SaveChangesAsync(cancellationToken);
    }
}
