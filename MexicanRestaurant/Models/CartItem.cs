using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MexicanRestaurant.Models;

public class CartItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public int MenuItemId { get; set; }

    [ForeignKey("MenuItemId")]
    public MenuItem? MenuItem { get; set; }

    public int Quantity { get; set; } = 1;

    public DateTime AddedAt { get; set; } = DateTime.Now;
}