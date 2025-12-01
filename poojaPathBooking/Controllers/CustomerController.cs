namespace poojaPathBooking.Controllers;

using Microsoft.AspNetCore.Mvc;
using poojaPathBooking.Models.Entities;
using poojaPathBooking.Models.DTOs;
using poojaPathBooking.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class CustomerController(ICustomerService customerService, ILogger<CustomerController> logger) : ControllerBase
{
    private readonly ICustomerService _customerService = customerService;
    private readonly ILogger<CustomerController> _logger = logger;

    /// <summary>
    /// Retrieves all customers from the database
    /// </summary>
    /// <returns>A list of all customers</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Customer>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers()
    {
        try
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all customers");
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
    }

    /// <summary>
    /// Retrieves a specific customer by their unique ID
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the customer</param>
    /// <returns>The customer details if found</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Customer), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Customer>> GetCustomerById(Guid id)
    {
        try
        {
            _logger.LogInformation("Retrieving customer with ID {Id}", id);
            var customer = await _customerService.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return NotFound(new { message = $"Customer with ID {id} not found" });
            }

            return Ok(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer with ID {Id}", id);
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
    }

    /// <summary>
    /// Searches for a customer by email address
    /// </summary>
    /// <param name="email">The email address to search for</param>
    /// <returns>The customer with matching email if found</returns>
    [HttpGet("search/email/{email}")]
    [ProducesResponseType(typeof(Customer), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Customer>> GetCustomerByEmail(string email)
    {
        try
        {
            var customer = await _customerService.GetCustomerByEmailAsync(email);

            if (customer == null)
            {
                return NotFound(new { message = $"Customer with email {email} not found" });
            }

            return Ok(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer with email {Email}", email);
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
    }

    /// <summary>
    /// Searches for a customer by contact number
    /// </summary>
    /// <param name="contactNumber">The contact number to search for</param>
    /// <returns>The customer with matching contact number if found</returns>
    [HttpGet("search/contact/{contactNumber}")]
    [ProducesResponseType(typeof(Customer), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Customer>> GetCustomerByContact(string contactNumber)
    {
        try
        {
            var customer = await _customerService.GetCustomerByContactAsync(contactNumber);

            if (customer == null)
            {
                return NotFound(new { message = $"Customer with contact number {contactNumber} not found" });
            }

            return Ok(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer with contact {Contact}", contactNumber);
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
    }

    /// <summary>
    /// Creates a new customer in the system
    /// </summary>
    /// <param name="dto">The customer details including name, contact information, and location</param>
    /// <returns>The newly created customer with assigned ID and timestamps</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Customer), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Customer>> CreateCustomer([FromBody] CreateCustomerDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _customerService.CreateCustomerAsync(dto);
            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.CustomerId }, customer);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Duplicate customer error");
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error creating customer");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
    }

    /// <summary>
    /// Updates an existing customer's information
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the customer to update</param>
    /// <param name="dto">The updated customer details</param>
    /// <returns>The updated customer</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Customer), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateCustomerById(Guid id, [FromBody] UpdateCustomerDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _customerService.UpdateCustomerAsync(id, dto);

            if (customer == null)
            {
                return NotFound(new { message = $"Customer with ID {id} not found" });
            }

            return Ok(customer);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Duplicate customer error");
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error updating customer");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer with ID {Id}", id);
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
    }

    /// <summary>
    /// Permanently deletes a customer from the database
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the customer to delete</param>
    /// <returns>A confirmation message if deletion is successful</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCustomerById(Guid id)
    {
        try
        {
            _logger.LogInformation("Deleting customer with ID {Id}", id);
            var result = await _customerService.DeleteCustomerAsync(id);

            if (!result)
            {
                return NotFound(new { message = $"Customer with ID {id} not found" });
            }

            return Ok(new { message = $"Customer has been deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer with ID {Id}", id);
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
    }
}
