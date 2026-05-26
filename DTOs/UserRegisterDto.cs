using System.ComponentModel.DataAnnotations;

namespace Food_order_Backend.DTOs;

public class UserRegisterDto
{
    [Required(ErrorMessage = "FullName is required")]
    [StringLength(100)]
    public required string FullName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(150)]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    [StringLength(255)]
    public required string Password { get; set; }
}
