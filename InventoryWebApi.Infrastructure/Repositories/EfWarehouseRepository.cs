using InventoryWebApi.Domain.Entities;
using InventoryWebApi.Infrastructure.Persistence;

namespace InventoryWebApi.Infrastructure.Repositories;

public sealed class EfWarehouseRepository : EfRepository<Warehouse>
{
    public EfWarehouseRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }
}
