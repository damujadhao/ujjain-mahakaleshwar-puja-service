namespace poojaPathBooking.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using poojaPathBooking.Models.DTOs;
using poojaPathBooking.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class CustomerAuthController : ControllerBase
{
    private readonly ICustomerAuthService _customerAuthService;
    private readonly ILogger<CustomerAuthController> _logger;

    public CustomerAuthController(ICustomerAuthService customerAuthService, ILogger<CustomerAuthController> logger)
    {
        _customerAuthService = customerAuthService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a customer using email or contact number and returns a JWT token
    /// </summary>
    /// <param name="dto">Login credentials (email/contact and password)</param>
    /// <returns>JWT token and customer information if authentication is successful</returns>
    /// <remarks>
    /// You can login using either:
    /// - Email address (e.g., "customer@example.com")
    /// - Contact number (e.g., "9876543210")
    /// </remarks>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomerAuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerAuthResponseDto>> Login([FromBody] CustomerLoginDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _customerAuthService.LoginAsync(dto);

            if (result == null)
            {
                return Unauthorized(new 
                { 
                    message = "Invalid email/contact number or password. Please check your credentials and try again.",
                    hint = "You can login using either your email address or contact number"
                });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during customer login");
            return StatusCode(500, "An error occurred during login");
        }
    }

    /// <summary>
    /// Registers a new customer account
    /// </summary>
    /// <param name="dto">Customer registration details including name, email, contact, and password</param>
    /// <returns>JWT token and customer information if registration is successful</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomerAuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerAuthResponseDto>> Register([FromBody] CustomerRegisterDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _customerAuthService.RegisterAsync(dto);
            return CreatedAtAction(nameof(Login), result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Customer registration failed");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during customer registration");
            return StatusCode(500, "An error occurred during registration");
        }
    }

    /// <summary>
    /// Gets the current logged-in customer's profile information
    /// </summary>
    /// <returns>Customer profile details</returns>
    [HttpGet("profile")]
    [Authorize(Roles = "Customer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetProfile()
    {
        try
        {
            var customerIdClaim = User.FindFirst("CustomerId")?.Value;
            
            if (string.IsNullOrEmpty(customerIdClaim) || !Guid.TryParse(customerIdClaim, out var customerId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var customer = await _customerAuthService.GetCustomerByEmailOrContactAsync(customerIdClaim);

            if (customer == null)
            {
                return NotFound(new { message = "Customer not found" });
            }

            return Ok(new
            {
                customerId = customer.CustomerId,
                firstName = customer.FirstName,
                lastName = customer.LastName,
                email = customer.Email,
                contactNumber = customer.ContactNumber,
                country = customer.Country,
                state = customer.State,
                district = customer.District,
                isActive = customer.IsActive,
                createdAt = customer.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer profile");
            return StatusCode(500, "An error occurred while retrieving profile");
        }
    }
}
