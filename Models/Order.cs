using System.ComponentModel.DataAnnotations;

namespace Food_order_Backend.Models;

public class Order
{
    public int OrderId { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    public required string Title { get; set; }

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public required string Type { get; set; }  // "Dine-in" | "Takeaway" | "Delivery"

    [Required]
    public required string Status { get; set; } // "Pending" | "Completed" | "Cancelled"

    [Range(0, 999999)]
    public decimal TotalPrice { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User? Customer { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
