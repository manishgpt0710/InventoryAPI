using InventoryWebApi.Domain.Entities;
using InventoryWebApi.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryWebApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IGenericService<Product> _genericService;

    public ProductsController(IGenericService<Product> genericService)
    {
        _genericService = genericService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll(int? pageNumber, int? pageSize, CancellationToken cancellationToken)
    {
        var products = await _genericService.GetAllAsync(pageNumber, pageSize, cancellationToken);
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await _genericService.GetByIdAsync(id, cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(Product product, CancellationToken cancellationToken)
    {
        var created = await _genericService.CreateAsync(product, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Product product, CancellationToken cancellationToken)
    {
        if (id != product.Id)
        {
            return BadRequest("Route id and body ProductId must match.");
        }

        var existing = await _genericService.GetByIdAsync(id, cancellationToken);
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

        await _genericService.UpdateAsync(existing, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var existing = await _genericService.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        await _genericService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}
