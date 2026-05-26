namespace Food_order_Backend.Models;

public class Order
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public required string Status { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public User? Customer { get; set; }
}