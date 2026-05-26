using Food_order_Backend.Data;
using Food_order_Backend.Models;
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

    // GET all products: api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        return await _context.Products.ToListAsync();
    }

    // GET 1 product: api/products/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if(product == null)
        {
            return NotFound(new
            {
                message = "Product not found"
            });
        }

        return Ok(product);
    }

    // CREATE product: api/products
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        _context.Products.Add(product);

        await _context.SaveChangesAsync();

        // return Ok(product);
        return Ok(new
        {
            message = "Product added successfully",
            data = product
        });
    }

    // UPDATE product: api/products/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        if (id != product.ProductId)
        {
            return BadRequest(new
            {
                message = "Product ID does not match"
            });
        }

        var existingProduct = await _context.Products.FindAsync(id);

        if (existingProduct == null)
        {
            return NotFound(new
            {
                message = "Product not found"
            });
        }

        existingProduct.ProductName = product.ProductName;
        existingProduct.Price = product.Price;
        existingProduct.CategoryId = product.CategoryId;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Product updated successfully",
            data = existingProduct
        });
    }

    // DELETE product: api/products/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return NotFound(new
            {
                message = "Product not found"
            });
        }

        _context.Products.Remove(product);

        await _context.SaveChangesAsync();

        // return NoContent();
        return Ok(new
        {
            message = "Product deleted successfully",
        });
    }
}