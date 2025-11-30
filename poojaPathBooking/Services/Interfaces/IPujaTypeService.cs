namespace poojaPathBooking.Services.Interfaces;

using poojaPathBooking.Models.Entities;
using poojaPathBooking.Models.DTOs;

public interface IPujaTypeService
{
    Task<IEnumerable<PujaType>> GetAllPujaTypesAsync();
    Task<PujaType?> GetPujaTypeByIdAsync(int id);
    Task<IEnumerable<PujaType>> GetActivePujaTypesAsync();
    Task<PujaType> CreatePujaTypeAsync(CreatePujaTypeDto dto);
    Task<PujaType?> UpdatePujaTypeAsync(int id, UpdatePujaTypeDto dto);
    Task<bool> DeletePujaTypeAsync(int id);
    Task<bool> DeactivatePujaTypeAsync(int id);
    Task<bool> PujaTypeExistsAsync(int id);
}
