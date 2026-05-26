using Food_order_Backend.Data;
using Food_order_Backend.Models;
using Food_order_Backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Food_order_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDBContext _context;

    public UsersController(AppDBContext context)
    {
        _context = context;
    }

    // POST api/users/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Password == dto.Password);

        if (user == null)
            return Unauthorized(new { message = "Invalid email or password" });

        return Ok(new
        {
            message = "Login successful",
            data = new
            {
                user.UserId,
                user.FullName,
                user.Email,
                user.Role
            }
        });
    }

    // POST api/users/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto dto)
    {
        var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
        if (emailExists)
            return Conflict(new { message = "Email already in use" });

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            Password = dto.Password, // NOTE: hash ใน production จริง
            Role = "Customer"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Registered successfully",
            data = new { user.UserId, user.FullName, user.Email, user.Role }
        });
    }

    // GET api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetUsers()
    {
        var users = await _context.Users
            .Select(u => new { u.UserId, u.FullName, u.Email, u.Role })
            .ToListAsync();
        return Ok(users);
    }

    // GET api/users/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound(new { message = "User not found" });

        return Ok(new { user.UserId, user.FullName, user.Email, user.Role });
    }

    // PUT api/users/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UserRegisterDto dto)
    {
        var existingUser = await _context.Users.FindAsync(id);
        if (existingUser == null)
            return NotFound(new { message = "User not found" });

        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == dto.Email && u.UserId != id);
        if (emailExists)
            return Conflict(new { message = "Email already in use" });

        existingUser.FullName = dto.FullName;
        existingUser.Email = dto.Email;
        existingUser.Password = dto.Password;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "User updated successfully",
            data = new { existingUser.UserId, existingUser.FullName, existingUser.Email, existingUser.Role }
        });
    }

    // DELETE api/users/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound(new { message = "User not found" });

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "User deleted successfully" });
    }
}
