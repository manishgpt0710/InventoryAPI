using Microsoft.Extensions.DependencyInjection;
using InventoryWebApi.Application.Services;
using InventoryWebApi.Application.Interfaces;
using InventoryWebApi.Application.Models;
using Microsoft.Extensions.Configuration;

namespace InventoryWebApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
        services.Configure<LocalStorageOptions>(configuration.GetSection("LocalStorage"));
        services.AddScoped<IImageStorage, LocalImageStorage>();

        return services;
    }
}
