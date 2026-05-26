using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Food_order_Backend.Data;
using Food_order_Backend.DTOs;

namespace Food_order_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // Dashboard — Admin เท่านั้น
public class DashboardController : ControllerBase
{
    private readonly AppDBContext _context;

    public DashboardController(AppDBContext context)
    {
        _context = context;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var totalOrders = await _context.Orders.CountAsync();
        var pendingOrders = await _context.Orders.CountAsync(o => o.Status == "Pending");
        var completedOrders = await _context.Orders.CountAsync(o => o.Status == "Completed");
        var cancelledOrders = await _context.Orders.CountAsync(o => o.Status == "Cancelled");
        var totalSales = await _context.Orders
            .Where(o => o.Status == "Completed")
            .SumAsync(o => (decimal?)o.TotalPrice) ?? 0;

        var recentOrders = await _context.Orders
            .Include(o => o.Customer)
            .OrderByDescending(o => o.CreatedAt)
            .Take(10)
            .Select(o => new OrderResponseDto
            {
                OrderId = o.OrderId,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer != null ? o.Customer.FullName : "Unknown",
                Title = o.Title,
                Status = o.Status,
                Type = o.Type,
                TotalPrice = o.TotalPrice,
                CreatedAt = o.CreatedAt
            })
            .ToListAsync();

        var topProducts = await _context.OrderItems
            .Include(oi => oi.Product)
            .GroupBy(oi => new { oi.ProductId, oi.Product!.ProductName })
            .Select(g => new
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.ProductName,
                TotalQuantity = g.Sum(oi => oi.Quantity),
                TotalRevenue = g.Sum(oi => oi.UnitPrice * oi.Quantity)
            })
            .OrderByDescending(x => x.TotalQuantity)
            .Take(5)
            .ToListAsync();

        return Ok(new
        {
            TotalOrders = totalOrders,
            PendingOrders = pendingOrders,
            CompletedOrders = completedOrders,
            CancelledOrders = cancelledOrders,
            TotalSales = totalSales,
            RecentOrders = recentOrders,
            TopProducts = topProducts
        });
    }
}
