using InventoryWebApi.Domain.Entities;
using InventoryWebApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryWebApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LookupItemsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public LookupItemsController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LookupItem>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await _dbContext.LookupItems
            .AsNoTracking()
            .Include(i => i.Group)
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LookupItem>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _dbContext.LookupItems
            .AsNoTracking()
            .Include(i => i.Group)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        if (item is null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<LookupItem>> Create(LookupItem item, CancellationToken cancellationToken)
    {
        item.CreatedAt = DateTime.UtcNow;

        _dbContext.LookupItems.Add(item);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, LookupItem item, CancellationToken cancellationToken)
    {
        if (id != item.Id)
        {
            return BadRequest("Route id and body Id must match.");
        }

        var existing = await _dbContext.LookupItems.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        existing.Name = item.Name;
        existing.Description = item.Description;
        existing.GroupId = item.GroupId;
        existing.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var existing = await _dbContext.LookupItems.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        _dbContext.LookupItems.Remove(existing);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
