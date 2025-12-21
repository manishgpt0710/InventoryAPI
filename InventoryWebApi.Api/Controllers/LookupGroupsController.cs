using InventoryWebApi.Application.Interfaces;
using InventoryWebApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace InventoryWebApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LookupGroupsController(IGenericService<LookupGroup> genericService) : ControllerBase
{
    private readonly IGenericService<LookupGroup> _genericService = genericService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LookupGroup>>> GetAll(int? pageNumber, int? pageSize, CancellationToken cancellationToken)
    {
        var groups = await _genericService.GetAllAsync(pageNumber, pageSize, cancellationToken);
        return Ok(groups);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LookupGroup>> GetById(int id, CancellationToken cancellationToken)
    {
        var group = await _genericService.GetByIdAsync(id, cancellationToken);
        if (group is null)
        {
            return NotFound();
        }

        return Ok(group);
    }

    [HttpPost]
    public async Task<ActionResult<LookupGroup>> Create(LookupGroup group, CancellationToken cancellationToken)
    {
        var created = await _genericService.CreateAsync(group, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, LookupGroup group, CancellationToken cancellationToken)
    {
        if (id != group.Id)
        {
            return BadRequest("Route id and body Id must match.");
        }

        var existing = await _genericService.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        existing.Name = group.Name;
        existing.Description = group.Description;

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
