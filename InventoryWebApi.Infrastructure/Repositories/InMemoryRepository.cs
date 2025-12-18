using InventoryWebApi.Application.Interfaces;

namespace InventoryWebApi.Infrastructure.Repositories;

public class InMemoryRepository<T> : IRepository<T> where T : class
{
    private const int DefaultPageSize = 50;
    private const int MaxPageSize = 200;

    private readonly Dictionary<int, T> _store = new();
    private readonly Func<T, int> _getId;
    private readonly Action<T, int> _setId;

    public InMemoryRepository(Func<T, int> getId, Action<T, int> setId)
    {
        _getId = getId;
        _setId = setId;
    }

    public Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _store.TryGetValue(id, out var entity);
        return Task.FromResult(entity);
    }

    public Task<IReadOnlyList<T>> GetAllAsync(int? pageNumber = 1, int? pageSize = DefaultPageSize, CancellationToken cancellationToken = default)
    {
        var result = _store.Values
            .OrderBy(_getId)
            .ToList();

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

            result = result
                .Skip((normalizedPageNumber - 1) * normalizedPageSize)
                .Take(normalizedPageSize)
                .ToList();
        }

        return Task.FromResult<IReadOnlyList<T>>(result);
    }

    public Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        var id = _getId(entity);
        if (id == 0)
        {
            _setId(entity, id + 1);
        }

        _store[id] = entity;
        return Task.FromResult(entity);
    }

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var id = _getId(entity);
        if (id == 0)
        {
            throw new InvalidOperationException("Entity must have a non-empty Id to be updated.");
        }

        _store[id] = entity;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        _store.Remove(id);
        return Task.CompletedTask;
    }
}
