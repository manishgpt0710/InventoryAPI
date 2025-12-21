using InventoryWebApi.Domain.Entities;
using InventoryWebApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryWebApi.Infrastructure.Repositories;

public class EfProductRepository : EfRepository<Product>
{
    public EfProductRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    protected override IQueryable<Product> ApplyGetByIdIncludes(IQueryable<Product> query)
    {
        return query.Include(p => p.Images)
                    .Include(p => p.WarehouseInventories);
    }

    public override async Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        DbContext.Set<ProductImage>().AddRange(product.Images);
        return await base.AddAsync(product, cancellationToken);
    }

    public override async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        DbContext.Set<ProductImage>().UpdateRange(product.Images);
        await base.UpdateAsync(product, cancellationToken);
    }
}
