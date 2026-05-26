using System.ComponentModel.DataAnnotations;

namespace Food_order_Backend.DTOs;

public class OrderCreateDto
{
    [Required(ErrorMessage = "CustomerId is required")]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    public required string Title { get; set; }

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Type is required")]
    public required string Type { get; set; } // "Dine-in" | "Takeaway" | "Delivery"

    // รายการสินค้าที่สั่ง (คำนวณ TotalPrice จาก items อัตโนมัติ)
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    [Required]
    public int ProductId { get; set; }

    [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
    public int Quantity { get; set; }
}
