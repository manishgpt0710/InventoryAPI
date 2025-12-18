using InventoryWebApi.Domain.Entities;
using InventoryWebApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryWebApi.Infrastructure.Repositories;

public sealed class EfLookupItemRepository : EfRepository<LookupItem>
{
    public EfLookupItemRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    protected override IQueryable<LookupItem> ApplyGetAllIncludes(IQueryable<LookupItem> query)
        => query.Include(i => i.Group);

    protected override IQueryable<LookupItem> ApplyGetByIdIncludes(IQueryable<LookupItem> query)
        => query.Include(i => i.Group);
}
