using InventoryWebApi.Application.Services;
using InventoryWebApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace InventoryWebApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehouseInventoriesController(IGenericService<WarehouseInventory> genericService) : ControllerBase
{
    private readonly IGenericService<WarehouseInventory> _genericService = genericService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WarehouseInventory>>> GetAll(int? pageNumber, int? pageSize, CancellationToken cancellationToken)
    {
        var inventories = await _genericService.GetAllAsync(pageNumber, pageSize, cancellationToken);
        return Ok(inventories);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WarehouseInventory>> GetById(int id, CancellationToken cancellationToken)
    {
        var inventory = await _genericService.GetByIdAsync(id, cancellationToken);

        if (inventory is null)
        {
            return NotFound();
        }

        return Ok(inventory);
    }

    [HttpPost]
    public async Task<ActionResult<WarehouseInventory>> Create(WarehouseInventory inventory, CancellationToken cancellationToken)
    {
        var created = await _genericService.CreateAsync(inventory, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, WarehouseInventory inventory, CancellationToken cancellationToken)
    {
        if (id != inventory.Id)
        {
            return BadRequest("Route id and body Id must match.");
        }

        var existing = await _genericService.GetByIdAsync(id, cancellationToken);
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
