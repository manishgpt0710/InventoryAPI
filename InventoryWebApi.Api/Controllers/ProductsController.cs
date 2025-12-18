using InventoryWebApi.Domain.Entities;
using InventoryWebApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryWebApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public ProductsController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll(CancellationToken cancellationToken)
    {
        var products = await _dbContext.Products.AsNoTracking().ToListAsync(cancellationToken);
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

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

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Product product, CancellationToken cancellationToken)
    {
        if (id != product.Id)
        {
            return BadRequest("Route id and body ProductId must match.");
        }

        var existing = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
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

        await _dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var existing = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        _dbContext.Products.Remove(existing);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
