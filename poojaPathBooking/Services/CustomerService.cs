namespace poojaPathBooking.Services;

using Microsoft.EntityFrameworkCore;
using poojaPathBooking.Data;
using poojaPathBooking.Models.Entities;
using poojaPathBooking.Models.DTOs;
using poojaPathBooking.Services.Interfaces;

public class CustomerService(ApplicationDbContext context, ILogger<CustomerService> logger) : ICustomerService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<CustomerService> _logger = logger;

    public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
    {
        try
        {
            return await _context.Customers
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all customers");
            throw;
        }
    }

    public async Task<Customer?> GetCustomerByIdAsync(Guid id)
    {
        try
        {
            return await _context.Customers.FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer with ID {Id}", id);
            throw;
        }
    }

    public async Task<Customer?> GetCustomerByEmailAsync(string email)
    {
        try
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer with email {Email}", email);
            throw;
        }
    }

    public async Task<Customer?> GetCustomerByContactAsync(string contactNumber)
    {
        try
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.ContactNumber == contactNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer with contact {Contact}", contactNumber);
            throw;
        }
    }

    public async Task<IEnumerable<Customer>> GetCustomersByStateAsync(string state)
    {
        try
        {
            return await _context.Customers
                .Where(c => c.State == state)
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers from state {State}", state);
            throw;
        }
    }

    public async Task<Customer> CreateCustomerAsync(CreateCustomerDto dto)
    {
        try
        {
            // Validate required fields
            ValidateCustomerDto(dto);

            // Check if email already exists
            if (!string.IsNullOrEmpty(dto.Email))
            {
                var existingCustomer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Email == dto.Email);

                if (existingCustomer != null)
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
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                ContactNumber = dto.ContactNumber.Trim(),
                Email = dto.Email?.Trim(),
                Country = dto.Country?.Trim(),
                State = dto.State?.Trim(),
                District = dto.District?.Trim(),
                CreatedAt = DateTime.Now
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return customer;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            throw;
        }
    }

    public async Task<Customer?> UpdateCustomerAsync(Guid id, UpdateCustomerDto dto)
    {
        try
        {
            // Validate required fields
            ValidateCustomerDto(dto);

            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return null;
            }

            // Check if email is being changed and if it already exists for another customer
            if (!string.IsNullOrEmpty(dto.Email) && dto.Email != customer.Email)
            {
                var existingCustomer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Email == dto.Email && c.CustomerId != id);

                if (existingCustomer != null)
                {
                    throw new InvalidOperationException("A customer with this email already exists");
                }
            }

            // Check if contact number is being changed and if it already exists for another customer
            if (dto.ContactNumber != customer.ContactNumber)
            {
                var existingContact = await _context.Customers
                    .FirstOrDefaultAsync(c => c.ContactNumber == dto.ContactNumber && c.CustomerId != id);

                if (existingContact != null)
                {
                    throw new InvalidOperationException("A customer with this contact number already exists");
                }
            }

            customer.FirstName = dto.FirstName.Trim();
            customer.LastName = dto.LastName.Trim();
            customer.ContactNumber = dto.ContactNumber.Trim();
            customer.Email = dto.Email?.Trim();
            customer.Country = dto.Country?.Trim();
            customer.State = dto.State?.Trim();
            customer.District = dto.District?.Trim();
            customer.UpdatedAt = DateTime.Now;

            _context.Entry(customer).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return customer;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            if (!await CustomerExistsAsync(id))
            {
                _logger.LogWarning("Customer with ID {Id} not found during update", id);
                return null;
            }
            _logger.LogError(ex, "Concurrency error updating customer with ID {Id}", id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer with ID {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteCustomerAsync(Guid id)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return false;
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer with ID {Id}", id);
            throw;
        }
    }

    public async Task<bool> CustomerExistsAsync(Guid id)
    {
        return await _context.Customers.AnyAsync(e => e.CustomerId == id);
    }

    private void ValidateCustomerDto(dynamic dto)
    {
        var errors = new List<string>();

        // Validate FirstName (required, max 120 characters)
        if (string.IsNullOrWhiteSpace(dto.FirstName))
        {
            errors.Add("FirstName is required and cannot be empty");
        }
        else if (dto.FirstName.Trim().Length > 120)
        {
            errors.Add("FirstName cannot exceed 120 characters");
        }
        else if (dto.FirstName.Trim().Length < 2)
        {
            errors.Add("FirstName must be at least 2 characters");
        }

        // Validate LastName (required, max 120 characters)
        if (string.IsNullOrWhiteSpace(dto.LastName))
        {
            errors.Add("LastName is required and cannot be empty");
        }
        else if (dto.LastName.Trim().Length > 120)
        {
            errors.Add("LastName cannot exceed 120 characters");
        }
        else if (dto.LastName.Trim().Length < 2)
        {
            errors.Add("LastName must be at least 2 characters");
        }

        // Validate ContactNumber (required, max 20 characters)
        if (string.IsNullOrWhiteSpace(dto.ContactNumber))
        {
            errors.Add("ContactNumber is required and cannot be empty");
        }
        else if (dto.ContactNumber.Trim().Length > 20)
        {
            errors.Add("ContactNumber cannot exceed 20 characters");
        }
        else if (dto.ContactNumber.Trim().Length < 10)
        {
            errors.Add("ContactNumber must be at least 10 digits");
        }

        // Validate Email (optional, max 320 characters, valid format)
        if (!string.IsNullOrEmpty(dto.Email))
        {
            if (dto.Email.Length > 320)
            {
                errors.Add("Email cannot exceed 320 characters");
            }
            else if (!IsValidEmail(dto.Email))
            {
                errors.Add("Email format is invalid");
            }
        }

        // Validate optional fields length
        if (!string.IsNullOrEmpty(dto.Country) && dto.Country.Length > 120)
        {
            errors.Add("Country cannot exceed 120 characters");
        }

        if (!string.IsNullOrEmpty(dto.State) && dto.State.Length > 120)
        {
            errors.Add("State cannot exceed 120 characters");
        }

        if (!string.IsNullOrEmpty(dto.District) && dto.District.Length > 120)
        {
            errors.Add("District cannot exceed 120 characters");
        }

        if (errors.Any())
        {
            var errorMessage = string.Join("; ", errors);
            _logger.LogWarning("Validation failed: {Errors}", errorMessage);
            throw new ArgumentException(errorMessage);
        }
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
