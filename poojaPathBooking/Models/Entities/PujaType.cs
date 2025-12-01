namespace poojaPathBooking.Models.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("PujaType", Schema = "dbo")]
public class PujaType
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PujaTypeId { get; set; }

    [Required]
    [MaxLength(500)]
    public string PujaTypeName { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; } = 0.00M;

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public string? BenefitOfPooja { get; set; }

    [MaxLength(100)]
    public string? PoojaDuration { get; set; }

    public string? RequiredThings { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}
