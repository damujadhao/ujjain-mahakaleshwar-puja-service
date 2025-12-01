namespace poojaPathBooking.Models.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Customer")]
public class Customer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid CustomerId { get; set; }

    [Required]
    [MaxLength(120)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(120)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string ContactNumber { get; set; } = string.Empty;

    [MaxLength(320)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(255)]
    public string? PasswordHash { get; set; }

    [MaxLength(120)]
    public string? Country { get; set; }

    [MaxLength(120)]
    public string? State { get; set; }

    [MaxLength(120)]
    public string? District { get; set; }

    [Required]
    public Int16 IsActive { get; set; } = 1;

    [Required]
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
