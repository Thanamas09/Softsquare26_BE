using Food_order_Backend.Data;
using Food_order_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Food_order_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDBContext _context;

    public CategoriesController(AppDBContext context)
    {
        _context = context;
    }

    // ทุกคนดูได้ — Public
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    {
        return await _context.Categories.ToListAsync();
    }

    // Admin เท่านั้น
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateCategory(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Category created", data = category });
    }

    // Admin เท่านั้น
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, Category category)
    {
        var existing = await _context.Categories.FindAsync(id);
        if (existing == null)
            return NotFound(new { message = "Category not found" });

        existing.CategoryName = category.CategoryName;
        await _context.SaveChangesAsync();
        return Ok(new { message = "Category updated", data = existing });
    }

    // Admin เท่านั้น
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var existing = await _context.Categories.FindAsync(id);
        if (existing == null)
            return NotFound(new { message = "Category not found" });

        _context.Categories.Remove(existing);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Category deleted" });
    }
}
