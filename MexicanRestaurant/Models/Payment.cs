using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MexicanRestaurant.Models;

public class Payment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int OrderId { get; set; }

    [ForeignKey("OrderId")]
    public Order? Order { get; set; }

    [StringLength(100)]
    public string? TransactionReference { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = "Pending";

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [StringLength(50)]
    public string? PaymentMethod { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.Now;
}