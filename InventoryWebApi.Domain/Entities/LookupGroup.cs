namespace InventoryWebApi.Domain.Entities;

public class LookupGroup : AuditableEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<LookupItem> Items { get; set; } = new List<LookupItem>();
}
