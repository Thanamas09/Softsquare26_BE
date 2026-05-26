using Food_order_Backend.Data;
using Food_order_Backend.Models;
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

    // GET all categories: api/categories
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    {
        return await _context.Categories.ToListAsync();
    }

    // GET 1 category: api/categories/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category == null)
        {
            return NotFound(new
            {
                message = "Category not found"
            });
        }

        return Ok(category);
    }

    // CREATE category: api/categories
    [HttpPost]
    public async Task<ActionResult<Category>> CreateCategory(Category category)
    {
        _context.Categories.Add(category);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Category added successfully",
            data = category
        });
    }

    // DELETE category: api/categories/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if(category == null)
        {
            return NotFound(new
            {
                message = "Category not found"
            });
        }

        _context.Categories.Remove(category);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Category deleted successfully"
        });
    }

}