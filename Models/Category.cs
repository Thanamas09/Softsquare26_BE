using System.ComponentModel.DataAnnotations;

namespace Food_order_Backend.Models;

public class Category
{
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "CategoryName is required")]
    [StringLength(100, ErrorMessage = "CategoryName cannot exceed 100 characters")]
    public required string CategoryName { get; set; }
}
