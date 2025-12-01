namespace poojaPathBooking.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using poojaPathBooking.Models.Entities;
using poojaPathBooking.Models.DTOs;
using poojaPathBooking.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class PujaTypeController(IPujaTypeService pujaTypeService, ILogger<PujaTypeController> logger) : ControllerBase
{
    private readonly IPujaTypeService _pujaTypeService = pujaTypeService;
    private readonly ILogger<PujaTypeController> _logger = logger;

    /// <summary>
    /// Retrieves all puja types from the database
    /// </summary>
    /// <returns>A list of all puja types including both active and inactive</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<PujaType>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PujaType>>> GetAllPujaTypes()
    {
        try
        {
            var pujaTypes = await _pujaTypeService.GetAllPujaTypesAsync();
            return Ok(pujaTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all puja types");
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
    }

    /// <summary>
    /// Retrieves a specific puja type by its ID
    /// </summary>
    /// <param name="id">The unique identifier of the puja type</param>
    /// <returns>The puja type details if found</returns>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PujaType), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PujaType>> GetPujaTypeById(int id)
    {
        try
        {
            _logger.LogInformation("Retrieving puja type with ID {Id}", id);
            var pujaType = await _pujaTypeService.GetPujaTypeByIdAsync(id);

            if (pujaType == null)
            {
                return NotFound(new { message = $"Puja type with ID {id} not found" });
            }

            return Ok(pujaType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving puja type with ID {Id}", id);
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
    }

    /// <summary>
    /// Retrieves only active puja types that are available for booking
    /// </summary>
    /// <returns>A list of active puja types where IsActive is true</returns>
    [HttpGet("active")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<PujaType>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PujaType>>> GetActivePujaTypes()
    {
        try
        {
            var activePujaTypes = await _pujaTypeService.GetActivePujaTypesAsync();
            return Ok(activePujaTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active puja types");
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
    }

    /// <summary>
    /// Creates a new puja type in the system (Admin only)
    /// </summary>
    /// <param name="dto">The puja type details including name, description, price, duration, benefits, and requirements</param>
    /// <returns>The newly created puja type with its assigned ID</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PujaType), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PujaType>> CreatePujaType([FromBody] CreatePujaTypeDto dto)
    {
        try
        {
            // Check if user is authenticated
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Unauthorized(new 
                { 
                    message = "You cannot add puja type data. Please login to continue.",
                    reason = "Authentication required"
                });
            }

            // Check if user has required role
            if (!User.IsInRole("Admin"))
            {
                return StatusCode(403, new 
                { 
                    message = "You don't have permission to add puja type data. Only Admin users can create puja types.",
                    reason = "Insufficient permissions",
                    requiredRoles = new[] { "Admin" },
                    yourRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "Unknown"
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pujaType = await _pujaTypeService.CreatePujaTypeAsync(dto);
            return CreatedAtAction(nameof(GetPujaTypeById), new { id = pujaType.PujaTypeId }, pujaType);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error creating puja type");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating puja type");
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
    }

    /// <summary>
    /// Updates an existing puja type with new information (Admin only)
    /// </summary>
    /// <param name="id">The unique identifier of the puja type to update</param>
    /// <param name="dto">The updated puja type details</param>
    /// <returns>The updated puja type</returns>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PujaType), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdatePujaTypeById(int id, [FromBody] UpdatePujaTypeDto dto)
    {
        _logger.LogInformation("UpdatePujaTypeById called with ID {Id}", id);
        try
        {
            // Check if user is authenticated
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Unauthorized(new 
                { 
                    message = "You cannot update puja type data. Please login to continue.",
                    reason = "Authentication required"
                });
            }

            // Check if user has required role
            if (!User.IsInRole("Admin"))
            {
                return StatusCode(403, new 
                { 
                    message = "You don't have permission to update puja type data. Only Admin users can update puja types.",
                    reason = "Insufficient permissions",
                    requiredRoles = new[] { "Admin" },
                    yourRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "Unknown"
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pujaType = await _pujaTypeService.UpdatePujaTypeAsync(id, dto);

            if (pujaType == null)
            {
                return NotFound(new { message = $"Puja type with ID {id} not found" });
            }

            return Ok(pujaType);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error updating puja type");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating puja type with ID {Id}", id);
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
    }

    /// <summary>
    /// Permanently deletes a puja type from the database (Admin only)
    /// </summary>
    /// <param name="id">The unique identifier of the puja type to delete</param>
    /// <returns>A confirmation message if deletion is successful</returns>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeletePujaTypeById(int id)
    {
        try
        {
            _logger.LogInformation("DeletePujaTypeById called with ID {Id}", id);
            // Check if user is authenticated
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Unauthorized(new 
                { 
                    message = "You cannot delete puja type data. Please login to continue.",
                    reason = "Authentication required"
                });
            }

            // Check if user has required role
            if (!User.IsInRole("Admin"))
            {
                return StatusCode(403, new 
                { 
                    message = "You don't have permission to delete puja type data. Only Admin users can delete puja types.",
                    reason = "Insufficient permissions",
                    requiredRoles = new[] { "Admin" },
                    yourRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "Unknown",
                    note = "If you want to temporarily disable a puja type, please use the deactivate endpoint instead."
                });
            }

            var result = await _pujaTypeService.DeletePujaTypeAsync(id);

            if (!result)
            {
                return NotFound(new { message = $"Puja type with ID {id} not found" });
            }

            return Ok(new { message = $"Puja type with ID {id} has been deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting puja type with ID {Id}", id);
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
    }

    /// <summary>
    /// Deactivates a puja type without deleting it (Admin only)
    /// </summary>
    /// <param name="id">The unique identifier of the puja type to deactivate</param>
    /// <returns>A confirmation message if deactivation is successful</returns>
    /// <remarks>
    /// This sets the IsActive flag to false, making the puja type unavailable for booking while preserving the data
    /// </remarks>
    [HttpPatch("{id:int}/deactivate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeactivatePujaType(int id)
    {
        try
        {
            // Check if user is authenticated
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Unauthorized(new 
                { 
                    message = "You cannot deactivate puja type data. Please login to continue.",
                    reason = "Authentication required"
                });
            }

            // Check if user has required role
            if (!User.IsInRole("Admin"))
            {
                return StatusCode(403, new 
                { 
                    message = "You don't have permission to deactivate puja type data. Only Admin users can deactivate puja types.",
                    reason = "Insufficient permissions",
                    requiredRoles = new[] { "Admin" },
                    yourRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "Unknown"
                });
            }

            var result = await _pujaTypeService.DeactivatePujaTypeAsync(id);

            if (!result)
            {
                return NotFound(new { message = $"Puja type with ID {id} not found" });
            }

            return Ok(new { message = $"Puja type with ID {id} has been deactivated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating puja type with ID {Id}", id);
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
    }
}
