using System.ComponentModel.DataAnnotations;

namespace Food_order_Backend.DTOs;

public class ProductCreateDto
{
    [Required(ErrorMessage = "ProductName is required")]
    [StringLength(200, ErrorMessage = "ProductName cannot exceed 200 characters")]
    public required string ProductName { get; set; }

    [Range(0, 99999, ErrorMessage = "Price must be between 0 and 99999")]
    public decimal Price { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public bool IsAvailable { get; set; } = true;
}
