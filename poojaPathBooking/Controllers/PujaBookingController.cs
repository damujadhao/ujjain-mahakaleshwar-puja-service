namespace poojaPathBooking.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using poojaPathBooking.Models.DTOs;
using poojaPathBooking.Models.Entities;
using poojaPathBooking.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class PujaBookingController(IPujaBookingService bookingService, ILogger<PujaBookingController> logger) : ControllerBase
{
    private readonly IPujaBookingService _bookingService = bookingService;
    private readonly ILogger<PujaBookingController> _logger = logger;

    /// <summary>
    /// Retrieves all puja bookings from the database (Admin/Manager only)
    /// </summary>
    /// <returns>A list of all bookings with customer and puja type details</returns>
    [HttpGet]
    //[Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(IEnumerable<PujaBookingResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PujaBookingResponseDto>>> GetAllBookings()
    {
        try
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all bookings");
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
    }

    /// <summary>
    /// Retrieves a specific booking by its ID
    /// </summary>
    /// <param name="id">The unique identifier of the booking</param>
    /// <returns>The booking details if found</returns>
    [HttpGet("{id:int}")]
    //[Authorize]
    [ProducesResponseType(typeof(PujaBookingResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PujaBookingResponseDto>> GetBookingById(int id)
    {
        try
        {
            _logger.LogInformation("Retrieving booking with ID {Id}", id);
            var booking = await _bookingService.GetBookingByIdAsync(id);

            if (booking == null)
            {
                return NotFound(new { message = $"Booking with ID {id} not found" });
            }

            return Ok(booking);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking with ID {Id}", id);
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
    }

    /// <summary>
    /// Retrieves all bookings for a specific customer
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer</param>
    /// <returns>A list of bookings for the specified customer</returns>
    [HttpGet("customer/{customerId:guid}")]
    //[Authorize]
    [ProducesResponseType(typeof(IEnumerable<PujaBookingResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PujaBookingResponseDto>>> GetBookingsByCustomerId(Guid customerId)
    {
        try
        {
            _logger.LogInformation("Retrieving bookings for customer {CustomerId}", customerId);
            var bookings = await _bookingService.GetBookingsByCustomerIdAsync(customerId);
            return Ok(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings for customer {CustomerId}", customerId);
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
    }

    /// <summary>
    /// Retrieves all bookings for a specific puja type
    /// </summary>
    /// <param name="pujaTypeId">The unique identifier of the puja type</param>
    /// <returns>A list of bookings for the specified puja type</returns>
    [HttpGet("puja-type/{pujaTypeId:int}")]
    //[Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(IEnumerable<PujaBookingResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PujaBookingResponseDto>>> GetBookingsByPujaTypeId(int pujaTypeId)
    {
        try
        {
            var bookings = await _bookingService.GetBookingsByPujaTypeIdAsync(pujaTypeId);
            return Ok(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings for puja type {PujaTypeId}", pujaTypeId);
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
    }

    /// <summary>
    /// Creates a new puja booking
    /// </summary>
    /// <param name="dto">The booking details</param>
    /// <returns>The newly created booking</returns>
    [HttpPost]
    //[Authorize]
    [ProducesResponseType(typeof(PujaBooking), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PujaBooking>> CreateBooking([FromBody] CreatePujaBookingDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var booking = await _bookingService.CreateBookingAsync(dto);
            return CreatedAtAction(nameof(GetBookingById), new { id = booking.BookingId }, booking);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error creating booking");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
    }

    /// <summary>
    /// Updates an existing booking
    /// </summary>
    /// <param name="id">The unique identifier of the booking to update</param>
    /// <param name="dto">The updated booking details</param>
    /// <returns>The updated booking</returns>
    [HttpPut("{id:int}")]
    //[Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(PujaBooking), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateBooking(int id, [FromBody] UpdatePujaBookingDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var booking = await _bookingService.UpdateBookingAsync(id, dto);

            if (booking == null)
            {
                return NotFound(new { message = $"Booking with ID {id} not found" });
            }

            return Ok(booking);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error updating booking");
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation updating booking");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking with ID {Id}", id);
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
    }

    /// <summary>
    /// Updates only the status of a booking
    /// </summary>
    /// <param name="id">The unique identifier of the booking</param>
    /// <param name="dto">The new status</param>
    /// <returns>Confirmation message if successful</returns>
    [HttpPatch("{id:int}/status")]
    //[Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] UpdateBookingStatusDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _bookingService.UpdateBookingStatusAsync(id, dto);

            if (!result)
            {
                return NotFound(new { message = $"Booking with ID {id} not found" });
            }

            return Ok(new { message = $"Booking status updated to {dto.BookingStatus} successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking status for ID {Id}", id);
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
    }

    /// <summary>
    /// Cancels a booking (sets status to Cancelled)
    /// </summary>
    /// <param name="id">The unique identifier of the booking to cancel</param>
    /// <returns>Confirmation message if successful</returns>
    [HttpPatch("{id:int}/cancel")]
    //[Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CancelBooking(int id)
    {
        try
        {
            var result = await _bookingService.CancelBookingAsync(id);

            if (!result)
            {
                return NotFound(new { message = $"Booking with ID {id} not found" });
            }

            return Ok(new { message = $"Booking with ID {id} has been cancelled successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation cancelling booking");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking with ID {Id}", id);
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
    }

    /// <summary>
    /// Permanently deletes a booking from the database (Admin only)
    /// </summary>
    /// <param name="id">The unique identifier of the booking to delete</param>
    /// <returns>Confirmation message if successful</returns>
    [HttpDelete("{id:int}")]
    //[Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        try
        {
            var result = await _bookingService.DeleteBookingAsync(id);

            if (!result)
            {
                return NotFound(new { message = $"Booking with ID {id} not found" });
            }

            return Ok(new { message = $"Booking with ID {id} has been deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking with ID {Id}", id);
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
    }
}
