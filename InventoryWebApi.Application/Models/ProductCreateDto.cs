using Microsoft.AspNetCore.Http;

namespace InventoryWebApi.Application.Models;

public class ProductCreateDto
{
    public string? SkuId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Uom { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ProductMetadata { get; set; }
    public bool IsBundled { get; set; }
    public decimal Rate { get; set; }
    public decimal Tax { get; set; }

    // single primary image upload (optional)
    public IFormFile? ImageFile { get; set; }
}
