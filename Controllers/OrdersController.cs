using Food_order_Backend.Data;
using Food_order_Backend.Models;
using Food_order_Backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Food_order_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // ทุก endpoint ต้อง Login ก่อน
public class OrdersController : ControllerBase
{
    private readonly AppDBContext _context;

    public OrdersController(AppDBContext context)
    {
        _context = context;
    }

    // Admin เท่านั้น — ดูออเดอร์ทั้งหมด
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrders()
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => MapToDto(o))
            .ToListAsync();

        return Ok(orders);
    }

    // Admin เท่านั้น — ดูออเดอร์เดี่ยว
    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponseDto>> GetOrder(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
            return NotFound(new { message = "Order not found" });

        return Ok(MapToDto(order));
    }

    // Customer — ดูออเดอร์ของตัวเอง (ดูได้เฉพาะ customerId ตัวเอง)
    [Authorize(Roles = "Customer,Admin")]
    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrdersByCustomer(int customerId)
    {
        // Customer ดูได้แค่ของตัวเอง, Admin ดูได้ทุกคน
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var currentRole = User.FindFirstValue(ClaimTypes.Role);

        if (currentRole == "Customer" && currentUserId != customerId)
            return Forbid();

        var customerExists = await _context.Users.AnyAsync(u => u.UserId == customerId);
        if (!customerExists)
            return NotFound(new { message = "Customer not found" });

        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => MapToDto(o))
            .ToListAsync();

        return Ok(orders);
    }

    // Customer — สั่งอาหาร
    [Authorize(Roles = "Customer,Admin")]
    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> CreateOrder(OrderCreateDto dto)
    {
        // Customer สั่งได้เฉพาะในนามตัวเอง
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var currentRole = User.FindFirstValue(ClaimTypes.Role);

        if (currentRole == "Customer" && currentUserId != dto.CustomerId)
            return Forbid();

        var customerExists = await _context.Users.AnyAsync(u => u.UserId == dto.CustomerId);
        if (!customerExists)
            return BadRequest(new { message = "Customer not found" });

        if (!dto.Items.Any())
            return BadRequest(new { message = "Order must have at least 1 item" });

        decimal totalPrice = 0;
        var orderItems = new List<OrderItem>();

        foreach (var item in dto.Items)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product == null)
                return BadRequest(new { message = $"Product ID {item.ProductId} not found" });
            if (!product.IsAvailable)
                return BadRequest(new { message = $"Product '{product.ProductName}' is not available" });

            totalPrice += product.Price * item.Quantity;
            orderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });
        }

        var order = new Order
        {
            CustomerId = dto.CustomerId,
            Title = dto.Title,
            Description = dto.Description,
            Type = dto.Type,
            Status = "Pending",
            TotalPrice = totalPrice,
            CreatedAt = DateTime.UtcNow,
            OrderItems = orderItems
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        var created = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .FirstAsync(o => o.OrderId == order.OrderId);

        return Ok(new { message = "Order created successfully", data = MapToDto(created) });
    }

    // Customer — ยกเลิกออเดอร์ตัวเอง
    [Authorize(Roles = "Customer,Admin")]
    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
            return NotFound(new { message = "Order not found" });

        // Customer ยกเลิกได้เฉพาะออเดอร์ตัวเอง
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var currentRole = User.FindFirstValue(ClaimTypes.Role);

        if (currentRole == "Customer" && order.CustomerId != currentUserId)
            return Forbid();

        if (order.Status == "Completed")
            return BadRequest(new { message = "Cannot cancel a completed order" });

        if (order.Status == "Cancelled")
            return BadRequest(new { message = "Order is already cancelled" });

        order.Status = "Cancelled";
        await _context.SaveChangesAsync();

        return Ok(new { message = "Order cancelled successfully", orderId = id, status = order.Status });
    }

    // Admin เท่านั้น — เปลี่ยน Status
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
    {
        var validStatuses = new[] { "Pending", "Completed", "Cancelled" };
        if (!validStatuses.Contains(dto.Status))
            return BadRequest(new { message = $"Invalid status. Valid: {string.Join(", ", validStatuses)}" });

        var order = await _context.Orders.FindAsync(id);
        if (order == null)
            return NotFound(new { message = "Order not found" });

        order.Status = dto.Status;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Status updated", orderId = id, status = order.Status });
    }

    // Admin เท่านั้น — แก้ไขออเดอร์
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, OrderCreateDto dto)
    {
        var existingOrder = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (existingOrder == null)
            return NotFound(new { message = "Order not found" });

        if (existingOrder.Status != "Pending")
            return BadRequest(new { message = "Only Pending orders can be edited" });

        decimal totalPrice = 0;
        var newItems = new List<OrderItem>();

        foreach (var item in dto.Items)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product == null)
                return BadRequest(new { message = $"Product ID {item.ProductId} not found" });

            totalPrice += product.Price * item.Quantity;
            newItems.Add(new OrderItem { ProductId = item.ProductId, Quantity = item.Quantity, UnitPrice = product.Price });
        }

        existingOrder.Title = dto.Title;
        existingOrder.Description = dto.Description;
        existingOrder.Type = dto.Type;
        existingOrder.CustomerId = dto.CustomerId;
        existingOrder.TotalPrice = totalPrice;

        _context.OrderItems.RemoveRange(existingOrder.OrderItems);
        existingOrder.OrderItems = newItems;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Order updated successfully" });
    }

    // Admin เท่านั้น — ลบออเดอร์
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
            return NotFound(new { message = "Order not found" });

        _context.OrderItems.RemoveRange(order.OrderItems);
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Order deleted successfully" });
    }

    private static OrderResponseDto MapToDto(Order o) => new()
    {
        OrderId = o.OrderId,
        CustomerId = o.CustomerId,
        CustomerName = o.Customer?.FullName ?? "Unknown",
        Title = o.Title,
        Description = o.Description,
        Status = o.Status,
        Type = o.Type,
        TotalPrice = o.TotalPrice,
        CreatedAt = o.CreatedAt,
        Items = o.OrderItems.Select(oi => new OrderItemResponseDto
        {
            OrderItemId = oi.OrderItemId,
            ProductId = oi.ProductId,
            ProductName = oi.Product?.ProductName ?? "",
            Quantity = oi.Quantity,
            UnitPrice = oi.UnitPrice
        }).ToList()
    };
}

public class UpdateStatusDto
{
    public required string Status { get; set; }
}
