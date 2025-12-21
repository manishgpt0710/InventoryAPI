namespace InventoryWebApi.Domain.Entities;

public class AuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

public class Product : AuditableEntity
{
    public int Id { get; set; }

    public string? SkuId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string ShortDescription { get; set; } = string.Empty;

    public string? Category { get; set; }

    public string? Uom { get; set; }

    public bool IsActive { get; set; } = true;

    public string? ProductMetadata { get; set; }

    public bool IsBundled { get; set; }

    public decimal Rate { get; set; } = 0.0m;

    public decimal Tax { get; set; } = 0.0m;

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

    public ICollection<WarehouseInventory> WarehouseInventories { get; set; } = new List<WarehouseInventory>();
}
