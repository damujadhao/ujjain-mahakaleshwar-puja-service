namespace poojaPathBooking.Services.Interfaces;

using poojaPathBooking.Models.Entities;
using poojaPathBooking.Models.DTOs;

public interface ICustomerService
{
    Task<IEnumerable<Customer>> GetAllCustomersAsync();
    Task<Customer?> GetCustomerByIdAsync(Guid id);
    Task<Customer?> GetCustomerByEmailAsync(string email);
    Task<Customer?> GetCustomerByContactAsync(string contactNumber);
    Task<IEnumerable<Customer>> GetCustomersByStateAsync(string state);
    Task<Customer> CreateCustomerAsync(CreateCustomerDto dto);
    Task<Customer?> UpdateCustomerAsync(Guid id, UpdateCustomerDto dto);
    Task<bool> DeleteCustomerAsync(Guid id);
    Task<bool> CustomerExistsAsync(Guid id);
}
