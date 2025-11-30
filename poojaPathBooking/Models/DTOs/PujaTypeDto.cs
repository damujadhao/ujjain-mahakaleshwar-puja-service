namespace poojaPathBooking.Models.DTOs;

using System.ComponentModel.DataAnnotations;

public class CreatePujaTypeDto
{
    [Required(ErrorMessage = "Puja type name is required")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "Puja type name must be between 3 and 150 characters")]
    public string PujaTypeName { get; set; } = string.Empty;

    [StringLength(int.MaxValue, ErrorMessage = "Description is too long")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0, 999999.99, ErrorMessage = "Price must be between 0 and 999,999.99")]
    public decimal Price { get; set; }

    [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
    [Url(ErrorMessage = "Invalid URL format")]
    public string? ImageUrl { get; set; }

    [StringLength(int.MaxValue, ErrorMessage = "Benefit of pooja is too long")]
    public string? BenefitOfPooja { get; set; }

    [StringLength(100, ErrorMessage = "Pooja duration cannot exceed 100 characters")]
    public string? PoojaDuration { get; set; }

    [StringLength(int.MaxValue, ErrorMessage = "Required things is too long")]
    public string? RequiredThings { get; set; }

    public bool IsActive { get; set; } = true;
}

public class UpdatePujaTypeDto
{
    [Required(ErrorMessage = "Puja type name is required")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "Puja type name must be between 3 and 150 characters")]
    public string PujaTypeName { get; set; } = string.Empty;

    [StringLength(int.MaxValue, ErrorMessage = "Description is too long")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0, 999999.99, ErrorMessage = "Price must be between 0 and 999,999.99")]
    public decimal Price { get; set; }

    [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
    [Url(ErrorMessage = "Invalid URL format")]
    public string? ImageUrl { get; set; }

    [StringLength(int.MaxValue, ErrorMessage = "Benefit of pooja is too long")]
    public string? BenefitOfPooja { get; set; }

    [StringLength(100, ErrorMessage = "Pooja duration cannot exceed 100 characters")]
    public string? PoojaDuration { get; set; }

    [StringLength(int.MaxValue, ErrorMessage = "Required things is too long")]
    public string? RequiredThings { get; set; }

    [Required(ErrorMessage = "IsActive status is required")]
    public bool IsActive { get; set; }
}