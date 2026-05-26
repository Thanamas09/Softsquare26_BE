using Food_order_Backend.Data;
using Food_order_Backend.Models;
using Food_order_Backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Food_order_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDBContext _context;

    public ProductsController(AppDBContext context)
    {
        _context = context;
    }

    // GET api/products  — ดูเมนูทั้งหมด (พร้อมชื่อ Category)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProducts()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Select(p => new ProductResponseDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Price = p.Price,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.CategoryName : "",
                ImageUrl = p.ImageUrl,
                IsAvailable = p.IsAvailable
            })
            .ToListAsync();

        return Ok(products);
    }

    // GET api/products/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponseDto>> GetProduct(int id)
    {
        var p = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductId == id);

        if (p == null)
            return NotFound(new { message = "Product not found" });

        return Ok(new ProductResponseDto
        {
            ProductId = p.ProductId,
            ProductName = p.ProductName,
            Price = p.Price,
            CategoryId = p.CategoryId,
            CategoryName = p.Category != null ? p.Category.CategoryName : "",
            ImageUrl = p.ImageUrl,
            IsAvailable = p.IsAvailable
        });
    }

    // POST api/products  — Admin: เพิ่มเมนู
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(ProductCreateDto dto)
    {
        var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId);
        if (!categoryExists)
            return BadRequest(new { message = "Category not found" });

        var product = new Product
        {
            ProductName = dto.ProductName,
            Price = dto.Price,
            CategoryId = dto.CategoryId,
            ImageUrl = dto.ImageUrl,
            IsAvailable = dto.IsAvailable
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Product added successfully", data = product });
    }

    // PUT api/products/{id}  — Admin: แก้ไขเมนู
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, ProductCreateDto dto)
    {
        var existingProduct = await _context.Products.FindAsync(id);
        if (existingProduct == null)
            return NotFound(new { message = "Product not found" });

        var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId);
        if (!categoryExists)
            return BadRequest(new { message = "Category not found" });

        existingProduct.ProductName = dto.ProductName;
        existingProduct.Price = dto.Price;
        existingProduct.CategoryId = dto.CategoryId;
        existingProduct.ImageUrl = dto.ImageUrl;
        existingProduct.IsAvailable = dto.IsAvailable;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Product updated successfully", data = existingProduct });
    }

    // DELETE api/products/{id}  — Admin: ลบเมนู
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound(new { message = "Product not found" });

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Product deleted successfully" });
    }
}
