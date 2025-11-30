namespace poojaPathBooking.Models.DTOs;

using System.ComponentModel.DataAnnotations;

public class CreateCustomerDto
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(120, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 120 characters")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(120, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 120 characters")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Contact number is required")]
    [StringLength(20, MinimumLength = 10, ErrorMessage = "Contact number must be between 10 and 20 characters")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    [RegularExpression(@"^[0-9+\-\s()]*$", ErrorMessage = "Contact number can only contain digits, +, -, spaces, and parentheses")]
    public string ContactNumber { get; set; } = string.Empty;

    [StringLength(320, ErrorMessage = "Email cannot exceed 320 characters")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }

    [StringLength(120, ErrorMessage = "Country cannot exceed 120 characters")]
    public string? Country { get; set; }

    [StringLength(120, ErrorMessage = "State cannot exceed 120 characters")]
    public string? State { get; set; }

    [StringLength(120, ErrorMessage = "District cannot exceed 120 characters")]
    public string? District { get; set; }
}

public class UpdateCustomerDto
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(120, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 120 characters")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(120, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 120 characters")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Contact number is required")]
    [StringLength(20, MinimumLength = 10, ErrorMessage = "Contact number must be between 10 and 20 characters")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    [RegularExpression(@"^[0-9+\-\s()]*$", ErrorMessage = "Contact number can only contain digits, +, -, spaces, and parentheses")]
    public string ContactNumber { get; set; } = string.Empty;

    [StringLength(320, ErrorMessage = "Email cannot exceed 320 characters")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }

    [StringLength(120, ErrorMessage = "Country cannot exceed 120 characters")]
    public string? Country { get; set; }

    [StringLength(120, ErrorMessage = "State cannot exceed 120 characters")]
    public string? State { get; set; }

    [StringLength(120, ErrorMessage = "District cannot exceed 120 characters")]
    public string? District { get; set; }
}
