using InventoryWebApi.Application.Interfaces;

namespace InventoryWebApi.Infrastructure.Repositories;

public class InMemoryRepository<T> : IRepository<T> where T : class
{
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

    public Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<T> result = _store.Values.ToList();
        return Task.FromResult(result);
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
