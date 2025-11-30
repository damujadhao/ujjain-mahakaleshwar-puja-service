// <copyright file="AuthDto.cs" company="Pooja Path Booking">
// Copyright (c) Pooja Path Booking. All rights reserved.
// </copyright>

namespace poojaPathBooking.Models.DTOs;

using System.ComponentModel.DataAnnotations;

public class LoginDto
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}

public class CustomerLoginDto
{
    [Required(ErrorMessage = "Email or Contact Number is required")]
    public string EmailOrContact { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;
}

public class RegisterDto
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(320)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required")]
    [RegularExpression("^(Admin|Manager|User)$", ErrorMessage = "Role must be Admin, Manager, or User")]
    public string Role { get; set; } = "User";
}

public class CustomerRegisterDto
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
    public string ContactNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(320)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;

    [StringLength(120)]
    public string? Country { get; set; }

    [StringLength(120)]
    public string? State { get; set; }

    [StringLength(120)]
    public string? District { get; set; }
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public class CustomerAuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
