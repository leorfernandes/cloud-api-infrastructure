using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryApi.Data;
using InventoryApi.Models;
using InventoryApi.Dtos;

namespace InventoryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly InventoryDbContext _context;

    public ItemsController(InventoryDbContext context)
    {
        _context = context;
    }

    // GET /api/items - includes Category and Owner (demonstrates a join)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReturnItemDto>>> GetItems()
    {
        return await _context.Items
            .Include(i => i.Category)
            .Include(i => i.Owner)
            .AsNoTracking()
            .Select(i => new ReturnItemDto
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                Quantity = i.Quantity,
                CategoryId = i.CategoryId,
                OwnerId = i.OwnerId
            })
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReturnItemDto>> GetItem(int id)
    {
        var item = await _context.Items
            .Include(i => i.Category)
            .Include(i => i.Owner)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item is null) return NotFound();
        return new ReturnItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Quantity = item.Quantity,
            CategoryId = item.CategoryId,
            OwnerId = item.OwnerId
        };
    }

    [HttpPost]
    public async Task<ActionResult<ReturnItemDto>> CreateItem(CreateItemDto itemDto)
    {
        var item = new Item
        {
            Name = itemDto.Name,
            Description = itemDto.Description,
            Quantity = itemDto.Quantity,
            CategoryId = itemDto.CategoryId,
            OwnerId = itemDto.OwnerId
        };

        _context.Items.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetItem), new { id = item.Id }, new ReturnItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Quantity = item.Quantity,
            CategoryId = item.CategoryId,
            OwnerId = item.OwnerId
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateItem(int id, UpdateItemDto updatedItem)
    {
        var item = await _context.Items.FindAsync(id);
        if (item is null) return NotFound();

        item.Name = updatedItem.Name;
        item.Description = updatedItem.Description;
        item.Quantity = updatedItem.Quantity;
        item.CategoryId = updatedItem.CategoryId;
        item.OwnerId = updatedItem.OwnerId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var item = await _context.Items.FindAsync(id);
        if (item is null) return NotFound();

        _context.Items.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
