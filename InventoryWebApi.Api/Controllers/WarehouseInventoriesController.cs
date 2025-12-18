using InventoryWebApi.Domain.Entities;
using InventoryWebApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryWebApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehouseInventoriesController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public WarehouseInventoriesController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WarehouseInventory>>> GetAll(CancellationToken cancellationToken)
    {
        var inventories = await _dbContext.WarehouseInventories
            .AsNoTracking()
            .Include(wi => wi.Product)
            .Include(wi => wi.Warehouse)
            .ToListAsync(cancellationToken);

        return Ok(inventories);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WarehouseInventory>> GetById(int id, CancellationToken cancellationToken)
    {
        var inventory = await _dbContext.WarehouseInventories
            .AsNoTracking()
            .Include(wi => wi.Product)
            .Include(wi => wi.Warehouse)
            .FirstOrDefaultAsync(wi => wi.Id == id, cancellationToken);

        if (inventory is null)
        {
            return NotFound();
        }

        return Ok(inventory);
    }

    [HttpPost]
    public async Task<ActionResult<WarehouseInventory>> Create(WarehouseInventory inventory, CancellationToken cancellationToken)
    {
        inventory.CreatedAt = DateTime.UtcNow;

        _dbContext.WarehouseInventories.Add(inventory);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = inventory.Id }, inventory);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, WarehouseInventory inventory, CancellationToken cancellationToken)
    {
        if (id != inventory.Id)
        {
            return BadRequest("Route id and body Id must match.");
        }

        var existing = await _dbContext.WarehouseInventories.FirstOrDefaultAsync(wi => wi.Id == id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        existing.ProductId = inventory.ProductId;
        existing.WarehouseId = inventory.WarehouseId;
        existing.TotalQty = inventory.TotalQty;
        existing.AvailableQty = inventory.AvailableQty;
        existing.ReservedQty = inventory.ReservedQty;
        existing.BlockedQty = inventory.BlockedQty;
        existing.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var existing = await _dbContext.WarehouseInventories.FirstOrDefaultAsync(wi => wi.Id == id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        _dbContext.WarehouseInventories.Remove(existing);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
