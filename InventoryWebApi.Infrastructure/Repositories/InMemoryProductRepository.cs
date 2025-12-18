using InventoryWebApi.Application.Interfaces;
using InventoryWebApi.Domain.Entities;

namespace InventoryWebApi.Infrastructure.Repositories;

public class InMemoryProductRepository : InMemoryRepository<Product>, IRepository<Product>
{
    public InMemoryProductRepository()
        : base(p => p.Id, (p, id) => p.Id = id)
    {
        // Seed some initial products into the in-memory store.
        _ = SeedInitialDataAsync();
    }

    private async Task SeedInitialDataAsync()
    {
        var existing = await GetAllAsync();
        if (existing.Count != 0)
        {
            return;
        }

        var samples = new List<Product>
        {
            new()
            {
                SkuId = "SKU-1001",
                ProductName = "Laptop 14\" i5 8GB",
                Category = "Electronics",
                Uom = "Unit",
                IsActive = true,
                ProductMetadata = "{\"brand\":\"Contoso\",\"model\":\"L14-i5\"}",
                IsBundled = false,
                Rate = 55000.00m,
                Tax = 18.00m
            },
            new()
            {
                SkuId = "SKU-1002",
                ProductName = "Wireless Mouse",
                Category = "Accessories",
                Uom = "Unit",
                IsActive = true,
                ProductMetadata = "{\"brand\":\"Contoso\",\"color\":\"Black\"}",
                IsBundled = false,
                Rate = 799.00m,
                Tax = 18.00m
            },
            new()
            {
                SkuId = "SKU-1003",
                ProductName = "Mechanical Keyboard",
                Category = "Accessories",
                Uom = "Unit",
                IsActive = true,
                ProductMetadata = "{\"switchType\":\"Blue\",\"layout\":\"ANSI\"}",
                IsBundled = false,
                Rate = 3499.00m,
                Tax = 18.00m
            },
            new()
            {
                SkuId = "SKU-1004",
                ProductName = "27\" 4K Monitor",
                Category = "Electronics",
                Uom = "Unit",
                IsActive = true,
                ProductMetadata = "{\"resolution\":\"3840x2160\",\"panel\":\"IPS\"}",
                IsBundled = false,
                Rate = 24999.00m,
                Tax = 18.00m
            },
            new()
            {
                SkuId = "SKU-1005",
                ProductName = "USB‑C Docking Station",
                Category = "Accessories",
                Uom = "Unit",
                IsActive = true,
                ProductMetadata = "{\"ports\":10}",
                IsBundled = false,
                Rate = 5999.00m,
                Tax = 18.00m
            },
            new()
            {
                SkuId = "SKU-1006",
                ProductName = "Office Chair Ergonomic",
                Category = "Furniture",
                Uom = "Unit",
                IsActive = true,
                ProductMetadata = "{\"color\":\"Grey\",\"armrest\":true}",
                IsBundled = false,
                Rate = 8999.00m,
                Tax = 18.00m
            },
            new()
            {
                SkuId = "SKU-1007",
                ProductName = "Standing Desk 120x60",
                Category = "Furniture",
                Uom = "Unit",
                IsActive = true,
                ProductMetadata = "{\"heightAdjustable\":true}",
                IsBundled = false,
                Rate = 17999.00m,
                Tax = 18.00m
            },
            new()
            {
                SkuId = "SKU-1008",
                ProductName = "External SSD 1TB",
                Category = "Storage",
                Uom = "Unit",
                IsActive = true,
                ProductMetadata = "{\"interface\":\"USB-C\",\"type\":\"NVMe\"}",
                IsBundled = false,
                Rate = 7499.00m,
                Tax = 18.00m
            },
            new()
            {
                SkuId = "SKU-1009",
                ProductName = "Cat6 Network Cable 10m",
                Category = "Cables",
                Uom = "Unit",
                IsActive = true,
                ProductMetadata = "{\"color\":\"Blue\"}",
                IsBundled = false,
                Rate = 599.00m,
                Tax = 18.00m
            },
            new()
            {
                SkuId = "SKU-1010",
                ProductName = "Combo: Wireless Keyboard + Mouse",
                Category = "Accessories",
                Uom = "Bundle",
                IsActive = true,
                ProductMetadata = "{\"items\":2}",
                IsBundled = true,
                Rate = 3999.00m,
                Tax = 18.00m
            },
            new()
            {
                SkuId = "SKU-1011",
                ProductName = "Enterprise Antivirus 1Y License",
                Category = "Software",
                Uom = "License",
                IsActive = true,
                ProductMetadata = "{\"durationMonths\":12}",
                IsBundled = false,
                Rate = 1299.00m,
                Tax = 18.00m
            }
        };

        foreach (var p in samples)
        {
            await AddAsync(p);
        }
    }
}
