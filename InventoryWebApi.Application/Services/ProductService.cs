using InventoryWebApi.Application.Interfaces;
using InventoryWebApi.Domain.Entities;

namespace InventoryWebApi.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        => _productRepository.GetAllAsync(cancellationToken);

    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _productRepository.GetByIdAsync(id, cancellationToken);

    public Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default)
        => _productRepository.AddAsync(product, cancellationToken);

    public Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
        => _productRepository.UpdateAsync(product, cancellationToken);

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => _productRepository.DeleteAsync(id, cancellationToken);
}
