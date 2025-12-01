namespace poojaPathBooking.Controllers;

using Microsoft.AspNetCore.Mvc;
using poojaPathBooking.Models.DTOs;
using poojaPathBooking.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly ILogger<AuthController> _logger = logger;

    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    /// <param name="dto">Login credentials (username and password)</param>
    /// <returns>JWT token and user information if authentication is successful</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(dto);

            if (result == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
    }

    /// <summary>
    /// Registers a new user (Admin only operation)
    /// </summary>
    /// <param name="dto">User registration details</param>
    /// <returns>JWT token and user information if registration is successful</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(dto);
            return CreatedAtAction(nameof(Login), result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Registration failed");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
    }

    /// <summary>
    /// Helper endpoint to generate password hash for testing (Development only)
    /// </summary>
    /// <param name="password">The password to hash</param>
    /// <returns>The hashed password</returns>
    [HttpGet("hash-password")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult HashPassword([FromQuery] string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return BadRequest(new { message = "Password is required" });
        }

        var hash = _authService.HashPassword(password);
        return Ok(new 
        { 
            password = password,
            hash = hash,
            sqlInsert = $"UPDATE dbo.[User] SET PasswordHash = '{hash}' WHERE Username = 'admin';"
        });
    }
}
