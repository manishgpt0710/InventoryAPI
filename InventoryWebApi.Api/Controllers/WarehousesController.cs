using InventoryWebApi.Domain.Entities;
using InventoryWebApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryWebApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehousesController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public WarehousesController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Warehouse>>> GetAll(CancellationToken cancellationToken)
    {
        var warehouses = await _dbContext.Warehouses.AsNoTracking().ToListAsync(cancellationToken);
        return Ok(warehouses);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Warehouse>> GetById(int id, CancellationToken cancellationToken)
    {
        var warehouse = await _dbContext.Warehouses.AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

        if (warehouse is null)
        {
            return NotFound();
        }

        return Ok(warehouse);
    }

    [HttpPost]
    public async Task<ActionResult<Warehouse>> Create(Warehouse warehouse, CancellationToken cancellationToken)
    {
        warehouse.CreatedAt = DateTime.UtcNow;

        _dbContext.Warehouses.Add(warehouse);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = warehouse.Id }, warehouse);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Warehouse warehouse, CancellationToken cancellationToken)
    {
        if (id != warehouse.Id)
        {
            return BadRequest("Route id and body Id must match.");
        }

        var existing = await _dbContext.Warehouses.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        existing.Name = warehouse.Name;
        existing.Address = warehouse.Address;
        existing.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var existing = await _dbContext.Warehouses.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        _dbContext.Warehouses.Remove(existing);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
