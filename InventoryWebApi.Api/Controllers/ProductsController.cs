using InventoryWebApi.Domain.Entities;
using InventoryWebApi.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryWebApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll(CancellationToken cancellationToken)
    {
        var products = await _productService.GetAllAsync(cancellationToken);
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(Product product, CancellationToken cancellationToken)
    {
        product.CreatedAt = DateTime.UtcNow;

        var created = await _productService.CreateAsync(product, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Product product, CancellationToken cancellationToken)
    {
        if (id != product.Id)
        {
            return BadRequest("Route id and body ProductId must match.");
        }

        var existing = await _productService.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        existing.SkuId = product.SkuId;
        existing.ProductName = product.ProductName;
        existing.Category = product.Category;
        existing.Uom = product.Uom;
        existing.IsActive = product.IsActive;
        existing.ProductMetadata = product.ProductMetadata;
        existing.IsBundled = product.IsBundled;
        existing.Rate = product.Rate;
        existing.Tax = product.Tax;
        existing.UpdatedAt = DateTime.UtcNow;

        await _productService.UpdateAsync(existing, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var existing = await _productService.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        await _productService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}
