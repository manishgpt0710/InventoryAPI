namespace InventoryWebApi.Domain.Entities;

public class LookupItem : AuditableEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int GroupId { get; set; }
    public LookupGroup? Group { get; set; }
}
