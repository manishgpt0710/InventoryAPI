namespace InventoryWebApi.Domain.Entities;

public class ProductImage : AuditableEntity
{
    public int Id { get; set; }
    public int ProductId { get; set; }

    public string ImageUrl { get; set; } = default!;
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }

    public Product Product { get; set; } = default!;
}