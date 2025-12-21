using InventoryWebApi.Infrastructure.Persistence;
using InventoryWebApi.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using InventoryWebApi.Application.Interfaces;
using InventoryWebApi.Domain.Entities;

namespace InventoryWebApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IRepository<Product>, EfProductRepository>();
        services.AddScoped<IRepository<LookupGroup>, EfLookupGroupRepository>();
        services.AddScoped<IRepository<LookupItem>, EfLookupItemRepository>();
        services.AddScoped<IRepository<Warehouse>, EfWarehouseRepository>();
        services.AddScoped<IRepository<WarehouseInventory>, EfWarehouseInventoryRepository>();

        return services;
    }
}
