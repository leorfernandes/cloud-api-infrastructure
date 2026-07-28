using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryApi.Data;
using InventoryApi.Models;
using InventoryApi.Dtos;

namespace InventoryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly InventoryDbContext _context;

    public UsersController(InventoryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReturnUserDto>>> GetUsers()
    {
        return await _context.Users.AsNoTracking().Select(u => new ReturnUserDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email
        }).ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReturnUserDto>> GetUser(int id)
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        if (user is null) return NotFound();
        return new ReturnUserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }

    [HttpPost]
    public async Task<ActionResult<ReturnUserDto>> CreateUser(CreateUserDto userDto)
    {
        var user = new User
        {
            Username = userDto.Username,
            Email = userDto.Email
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new ReturnUserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, UpdateUserDto updatedUser)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null) return NotFound();

        user.Username = updatedUser.Username;
        user.Email = updatedUser.Email;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null) return NotFound();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
