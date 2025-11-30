namespace poojaPathBooking.Services.Interfaces;

using poojaPathBooking.Models.DTOs;
using poojaPathBooking.Models.Entities;

public interface ICustomerAuthService
{
    Task<CustomerAuthResponseDto?> LoginAsync(CustomerLoginDto dto);
    Task<CustomerAuthResponseDto?> RegisterAsync(CustomerRegisterDto dto);
    Task<Customer?> GetCustomerByEmailOrContactAsync(string emailOrContact);
    string GenerateCustomerJwtToken(Customer customer);
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
