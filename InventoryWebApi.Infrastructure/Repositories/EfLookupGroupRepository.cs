using InventoryWebApi.Domain.Entities;
using InventoryWebApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryWebApi.Infrastructure.Repositories;

public sealed class EfLookupGroupRepository : EfRepository<LookupGroup>
{
    public EfLookupGroupRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    protected override IQueryable<LookupGroup> ApplyGetAllIncludes(IQueryable<LookupGroup> query)
        => query.AsSplitQuery().Include(g => g.Items);

    protected override IQueryable<LookupGroup> ApplyGetByIdIncludes(IQueryable<LookupGroup> query)
        => query.AsSplitQuery().Include(g => g.Items);
}
