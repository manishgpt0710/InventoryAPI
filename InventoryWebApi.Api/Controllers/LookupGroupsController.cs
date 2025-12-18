using InventoryWebApi.Domain.Entities;
using InventoryWebApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryWebApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LookupGroupsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public LookupGroupsController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LookupGroup>>> GetAll(CancellationToken cancellationToken)
    {
        var groups = await _dbContext.LookupGroups
            .AsNoTracking()
            .Include(g => g.Items)
            .ToListAsync(cancellationToken);

        return Ok(groups);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LookupGroup>> GetById(int id, CancellationToken cancellationToken)
    {
        var group = await _dbContext.LookupGroups
            .AsNoTracking()
            .Include(g => g.Items)
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

        if (group is null)
        {
            return NotFound();
        }

        return Ok(group);
    }

    [HttpPost]
    public async Task<ActionResult<LookupGroup>> Create(LookupGroup group, CancellationToken cancellationToken)
    {
        _dbContext.LookupGroups.Add(group);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = group.Id }, group);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, LookupGroup group, CancellationToken cancellationToken)
    {
        if (id != group.Id)
        {
            return BadRequest("Route id and body Id must match.");
        }

        var existing = await _dbContext.LookupGroups.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        existing.Name = group.Name;
        existing.Description = group.Description;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var existing = await _dbContext.LookupGroups.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        _dbContext.LookupGroups.Remove(existing);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
