using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MexicanRestaurant.Models;

public class Order
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; } = DateTime.Now;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Subtotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Tax { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = "Pending";

    [StringLength(200)]
    public string? ShippingAddress { get; set; }

    [StringLength(100)]
    public string? CustomerName { get; set; }

    [StringLength(100)]
    public string? CustomerEmail { get; set; }

    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}