using Food_order_Backend.Data;
using Food_order_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Food_order_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly AppDBContext _context;

    public OrdersController(AppDBContext context)
    {
        _context = context;
    }

    // ดึงออเดอร์ทั้งหมด
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
    {
        // ใช้ .Include() เพื่อดึงชื่อและข้อมูลลูกค้ามาพร้อมกับออเดอร์เลย
        return await _context.Orders
            .Include(o => o.Customer) 
            .ToListAsync();
    }

    // ดึงออเดอร์เดียวตาม ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrder(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
        {
            return NotFound(new { message = "Order not found" });
        }

        return Ok(order);
    }

    // สร้างออเดอร์ใหม่
    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(Order order)
    {
        order.CreatedAt = DateTime.UtcNow; // เซ็ตเวลาที่สร้างออเดอร์อัตโนมัติ
        
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Order created successfully",
            data = order
        });
    }

    // อัปเดตออเดอร์
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, Order order)
    {
        if (id != order.OrderId)
        {
            return BadRequest(new { message = "Order ID does not match" });
        }

        var existingOrder = await _context.Orders.FindAsync(id);

        if (existingOrder == null)
        {
            return NotFound(new { message = "Order not found" });
        }

        existingOrder.Title = order.Title;
        existingOrder.Description = order.Description;
        existingOrder.Type = order.Type;
        existingOrder.Status = order.Status;
        existingOrder.TotalPrice = order.TotalPrice;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Order updated successfully",
            data = existingOrder
        });
    }

    // ลบออเดอร์
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
        {
            return NotFound(new { message = "Order not found" });
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Order deleted successfully" });
    }
}