namespace InventoryWebApi.Domain.Entities;

public class WarehouseInventory : AuditableEntity
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public int WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }

    public int TotalQty { get; set; }
    public int AvailableQty { get; set; }
    public int ReservedQty { get; set; }
    public int BlockedQty { get; set; }
}
