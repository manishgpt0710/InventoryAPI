using InventoryWebApi.Application.Interfaces;

namespace InventoryWebApi.Application.Services;

public class GenericService<T> : IGenericService<T> where T : class
{
    private readonly IRepository<T> _repository;

    public GenericService(IRepository<T> repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<T>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken cancellationToken = default)
        => _repository.GetAllAsync(pageNumber, pageSize, cancellationToken);

    public Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _repository.GetByIdAsync(id, cancellationToken);

    public Task<T> CreateAsync(T product, CancellationToken cancellationToken = default)
        => _repository.AddAsync(product, cancellationToken);

    public Task UpdateAsync(T product, CancellationToken cancellationToken = default)
        => _repository.UpdateAsync(product, cancellationToken);

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => _repository.DeleteAsync(id, cancellationToken);
}
