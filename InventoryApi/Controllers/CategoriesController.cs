using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryApi.Data;
using InventoryApi.Models;
using InventoryApi.Dtos;

namespace InventoryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly InventoryDbContext _context;

    public CategoriesController(InventoryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReturnCategoryDto>>> GetCategories()
    {
        return await _context.Categories.AsNoTracking().Select(c => new ReturnCategoryDto
        {
            Id = c.Id,
            Name = c.Name
        }).ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReturnCategoryDto>> GetCategory(int id)
    {
        var category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        if (category is null) return NotFound();
        return new ReturnCategoryDto
        {
            Id = category.Id,
            Name = category.Name
        };
    }

    [HttpPost]
    public async Task<ActionResult<ReturnCategoryDto>> CreateCategory(CreateCategoryDto categoryDto)
    {
        var category = new Category
        {
            Name = categoryDto.Name
        };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, new ReturnCategoryDto
        {
            Id = category.Id,
            Name = category.Name
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category is null) return NotFound();

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto categoryDto)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category is null) return NotFound();

        category.Name = categoryDto.Name;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
