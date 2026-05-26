namespace Food_order_Backend.Models;

public class User
{
    public int UserId { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
}