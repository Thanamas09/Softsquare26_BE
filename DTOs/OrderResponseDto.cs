namespace Food_order_Backend.DTOs;

public class OrderResponseDto
{
    public int OrderId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public string Type { get; set; } = "Dine-in";
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CustomerName { get; set; } = string.Empty;
}