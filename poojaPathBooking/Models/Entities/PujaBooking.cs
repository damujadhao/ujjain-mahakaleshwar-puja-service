namespace poojaPathBooking.Models.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("PujaBooking", Schema = "dbo")]
public class PujaBooking
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BookingId { get; set; }

    [Required]
    public int PujaTypeId { get; set; }

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    [MaxLength(20)]
    public string BookingMode { get; set; } = string.Empty; // Online, Offline, Phone

    [Required]
    public DateOnly PujaDate { get; set; }

    public TimeOnly? PujaTime { get; set; }

    [Required]
    [Range(1, 1000)]
    public int PeopleCount { get; set; }

    public string? Note { get; set; }

    [Required]
    [MaxLength(30)]
    public string BookingStatus { get; set; } = "Pending"; // Pending, Confirmed, Completed, Cancelled

    [Required]
    [Column(TypeName = "decimal(12,2)")]
    public decimal TotalAmount { get; set; }

    [Required]
    [MaxLength(10)]
    public string Currency { get; set; } = "INR";

    [Required]
    public bool IsPaid { get; set; } = false;

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public DateTime? UpdatedDate { get; set; }

    // Navigation properties
    [ForeignKey(nameof(PujaTypeId))]
    public virtual PujaType? PujaType { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public virtual Customer? Customer { get; set; }
}
