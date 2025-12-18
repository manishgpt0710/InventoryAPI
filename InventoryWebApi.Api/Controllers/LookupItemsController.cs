using InventoryWebApi.Application.Services;
using InventoryWebApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace InventoryWebApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LookupItemsController(IGenericService<LookupItem> genericService) : ControllerBase
{
    private readonly IGenericService<LookupItem> _genericService = genericService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LookupItem>>> GetAll(int? pageNumber, int? pageSize, CancellationToken cancellationToken)
    {
        var items = await _genericService.GetAllAsync(pageNumber, pageSize, cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LookupItem>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _genericService.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<LookupItem>> Create(LookupItem item, CancellationToken cancellationToken)
    {
        var created = await _genericService.CreateAsync(item, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, LookupItem item, CancellationToken cancellationToken)
    {
        if (id != item.Id)
        {
            return BadRequest("Route id and body Id must match.");
        }

        var existing = await _genericService.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        existing.Name = item.Name;
        existing.Description = item.Description;
        existing.GroupId = item.GroupId;

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
