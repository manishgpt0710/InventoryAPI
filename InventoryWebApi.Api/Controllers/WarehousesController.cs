using InventoryWebApi.Application.Interfaces;
using InventoryWebApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace InventoryWebApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehousesController(IGenericService<Warehouse> dbContext) : ControllerBase
{
    private readonly IGenericService<Warehouse> _genericService = dbContext;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Warehouse>>> GetAll(int? pageNumber, int? pageSize, CancellationToken cancellationToken)
    {
        var warehouses = await _genericService.GetAllAsync(pageNumber, pageSize, cancellationToken);
        return Ok(warehouses);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Warehouse>> GetById(int id, CancellationToken cancellationToken)
    {
        var warehouse = await _genericService.GetByIdAsync(id, cancellationToken);
        if (warehouse is null)
        {
            return NotFound();
        }
        return Ok(warehouse);
    }

    [HttpPost]
    public async Task<ActionResult<Warehouse>> Create(Warehouse warehouse, CancellationToken cancellationToken)
    {
        var created = await _genericService.CreateAsync(warehouse, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Warehouse warehouse, CancellationToken cancellationToken)
    {
        if (id != warehouse.Id)
        {
            return BadRequest("Route id and body Id must match.");
        }

        var existing = await _genericService.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        existing.Name = warehouse.Name;
        existing.Address = warehouse.Address;

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
