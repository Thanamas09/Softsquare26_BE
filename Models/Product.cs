namespace Food_order_Backend.Models;

public class Product
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; } 
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
}