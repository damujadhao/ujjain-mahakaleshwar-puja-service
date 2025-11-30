namespace poojaPathBooking.Services.Interfaces;

using poojaPathBooking.Models.DTOs;
using poojaPathBooking.Models.Entities;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
    Task<User?> GetUserByUsernameAsync(string username);
    string GenerateJwtToken(User user);
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
