using Food_order_Backend.Data;
using Food_order_Backend.Models;
using Food_order_Backend.DTOs;
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
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrders()
    {
        // ใช้ .Select() เพื่อแปลงข้อมูลจาก Database ให้เป็น DTO ก่อนส่งกลับ
        var orders = await _context.Orders
            .Include(o => o.Customer) 
            .Select(o => new OrderResponseDto
            {
                OrderId = o.OrderId,
                Title = o.Title,
                Description = o.Description,
                Status = o.Status,
                Type = o.Type,
                TotalPrice = o.TotalPrice,
                CreatedAt = o.CreatedAt,
                CustomerName = o.Customer != null ? o.Customer.FullName : "Unknown" 
            })
            .ToListAsync();
            
        return Ok(orders);
    }

    // ดึงออเดอร์เดียวตาม ID
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponseDto>> GetOrder(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
        {
            return NotFound(new { message = "Order not found" });
        }

        // แปลงข้อมูลออเดอร์เดี่ยวให้เป็น DTO
        var responseDto = new OrderResponseDto
        {
            OrderId = order.OrderId,
            Title = order.Title,
            Description = order.Description,
            Status = order.Status,
            Type = order.Type,
            TotalPrice = order.TotalPrice,
            CreatedAt = order.CreatedAt,
            CustomerName = order.Customer != null ? order.Customer.FullName : "Unknown"
        };

        return Ok(responseDto);
    }

    // สร้างออเดอร์ใหม่
    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(OrderCreateDto dto)
    {
      var order = new Order
      {
          Title = dto.Title,
          Description = dto.Description,
          Type = dto.Type,
          CustomerId = dto.CustomerId,
          TotalPrice = dto.TotalPrice,
          Status = "Pending",
          CreatedAt = DateTime.UtcNow
      };
      
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
    public async Task<IActionResult> UpdateOrder(int id, OrderCreateDto dto)
    {
        var existingOrder = await _context.Orders.FindAsync(id);

        if (existingOrder == null)
        {
            return NotFound(new { message = "Order not found" });
        }

        existingOrder.Title = dto.Title;
        existingOrder.Description = dto.Description;
        existingOrder.Type = dto.Type;
        existingOrder.CustomerId = dto.CustomerId;
        existingOrder.TotalPrice = dto.TotalPrice;

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