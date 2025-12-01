namespace poojaPathBooking.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using poojaPathBooking.Data;
using poojaPathBooking.Models.DTOs;
using poojaPathBooking.Models.Entities;
using poojaPathBooking.Services.Interfaces;

public class CustomerAuthService : ICustomerAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CustomerAuthService> _logger;

    public CustomerAuthService(
        ApplicationDbContext context,
        IConfiguration configuration,
        ILogger<CustomerAuthService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<CustomerAuthResponseDto?> LoginAsync(CustomerLoginDto dto)
    {
        try
        {
            var customer = await GetCustomerByEmailOrContactAsync(dto.EmailOrContact);

            if (customer == null || !Convert.ToBoolean(customer.IsActive))
            {
                _logger.LogWarning("Login failed: Customer with email/contact {EmailOrContact} not found or inactive", dto.EmailOrContact);
                return null;
            }

            if (string.IsNullOrEmpty(customer.PasswordHash))
            {
                _logger.LogWarning("Login failed: Customer {EmailOrContact} does not have a password set", dto.EmailOrContact);
                return null;
            }

            if (!VerifyPassword(dto.Password, customer.PasswordHash))
            {
                _logger.LogWarning("Login failed: Invalid password for customer {EmailOrContact}", dto.EmailOrContact);
                return null;
            }

            var token = GenerateCustomerJwtToken(customer);
            var expiresAt = DateTime.UtcNow.AddHours(
                int.Parse(_configuration["Jwt:ExpiryHours"] ?? "24"));

            return new CustomerAuthResponseDto
            {
                Token = token,
                CustomerId = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email ?? string.Empty,
                ContactNumber = customer.ContactNumber,
                ExpiresAt = expiresAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during customer login for {EmailOrContact}", dto.EmailOrContact);
            throw;
        }
    }

    public async Task<CustomerAuthResponseDto?> RegisterAsync(CustomerRegisterDto dto)
    {
        try
        {
            // Check if email already exists
            if (!string.IsNullOrEmpty(dto.Email))
            {
                var existingEmail = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Email == dto.Email);
                if (existingEmail != null)
                {
                    throw new InvalidOperationException("A customer with this email already exists");
                }
            }

            // Check if contact number already exists
            var existingContact = await _context.Customers
                .FirstOrDefaultAsync(c => c.ContactNumber == dto.ContactNumber);
            if (existingContact != null)
            {
                throw new InvalidOperationException("A customer with this contact number already exists");
            }

            var customer = new Customer
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                ContactNumber = dto.ContactNumber,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                Country = dto.Country,
                State = dto.State,
                District = dto.District,
                IsActive = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CustomerId = Guid.NewGuid()
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var token = GenerateCustomerJwtToken(customer);
            var expiresAt = DateTime.UtcNow.AddHours(
                int.Parse(_configuration["Jwt:ExpiryHours"] ?? "24"));

            return new CustomerAuthResponseDto
            {
                Token = token,
                CustomerId = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email ?? string.Empty,
                ContactNumber = customer.ContactNumber,
                ExpiresAt = expiresAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during customer registration");
            throw;
        }
    }

    public async Task<Customer?> GetCustomerByEmailOrContactAsync(string emailOrContact)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == emailOrContact || c.ContactNumber == emailOrContact);
    }

    public string GenerateCustomerJwtToken(Customer customer)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, customer.CustomerId.ToString()),
            new Claim(ClaimTypes.Name, $"{customer.FirstName} {customer.LastName}"),
            new Claim(ClaimTypes.Email, customer.Email ?? string.Empty),
            new Claim(ClaimTypes.MobilePhone, customer.ContactNumber),
            new Claim(ClaimTypes.Role, "Customer"),
            new Claim("CustomerId", customer.CustomerId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(int.Parse(_configuration["Jwt:ExpiryHours"] ?? "24")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var hash = HashPassword(password);
        return hash == passwordHash;
    }
}
