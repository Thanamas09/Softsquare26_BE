namespace Food_order_Backend.DTOs;

public class OrderCreateDto
{
    public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public required string Type { get; set; }
    public int CustomerId { get; set; }
    public decimal TotalPrice { get; set; }
}