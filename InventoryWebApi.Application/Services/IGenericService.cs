using InventoryWebApi.Domain.Entities;

namespace InventoryWebApi.Application.Services;

public interface IGenericService<T> where T : class
{
    Task<IReadOnlyList<T>> GetAllAsync(int? pageNumber, int? pageSize, CancellationToken cancellationToken = default);

    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<T> CreateAsync(T record, CancellationToken cancellationToken = default);

    Task UpdateAsync(T reocrd, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
