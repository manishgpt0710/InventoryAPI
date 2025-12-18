using InventoryWebApi.Infrastructure.Persistence;
using InventoryWebApi.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using InventoryWebApi.Application.Interfaces;

namespace InventoryWebApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IProductRepository, EfProductRepository>();

        return services;
    }
}
