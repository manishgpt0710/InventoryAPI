namespace InventoryWebApi.Domain.Entities;

public class Warehouse : AuditableEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Address { get; set; }

    public ICollection<WarehouseInventory> WarehouseInventories { get; set; } = new List<WarehouseInventory>();
}
