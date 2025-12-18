using InventoryWebApi.Domain.Entities;
using InventoryWebApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryWebApi.Infrastructure.Repositories;

public sealed class EfWarehouseInventoryRepository : EfRepository<WarehouseInventory>
{
    public EfWarehouseInventoryRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    protected override IQueryable<WarehouseInventory> ApplyGetAllIncludes(IQueryable<WarehouseInventory> query)
        => query
            .Include(wi => wi.Product)
            .Include(wi => wi.Warehouse);

    protected override IQueryable<WarehouseInventory> ApplyGetByIdIncludes(IQueryable<WarehouseInventory> query)
        => query
            .Include(wi => wi.Product)
            .Include(wi => wi.Warehouse);
}
