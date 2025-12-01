namespace poojaPathBooking.Services;

using Microsoft.EntityFrameworkCore;
using poojaPathBooking.Data;
using poojaPathBooking.Models.Entities;
using poojaPathBooking.Models.DTOs;
using poojaPathBooking.Services.Interfaces;

public class PujaTypeService(ApplicationDbContext context, ILogger<PujaTypeService> logger) : IPujaTypeService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<PujaTypeService> _logger = logger;

    public async Task<IEnumerable<PujaType>> GetAllPujaTypesAsync()
    {
        try
        {
            return await _context.PujaTypes.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all puja types");
            throw;
        }
    }

    public async Task<PujaType?> GetPujaTypeByIdAsync(int id)
    {
        try
        {
            return await _context.PujaTypes.FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving puja type with ID {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<PujaType>> GetActivePujaTypesAsync()
    {
        try
        {
            return await _context.PujaTypes
                .Where(p => p.IsActive)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active puja types");
            throw;
        }
    }

    public async Task<PujaType> CreatePujaTypeAsync(CreatePujaTypeDto dto)
    {
        _logger.LogInformation("Creating new puja type for data : {@Dto}", dto);
        try
        {
            // Validate required fields
            ValidatePujaTypeDto(dto);

            var pujaType = new PujaType
            {
                PujaTypeName = dto.PujaTypeName.Trim(),
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                BenefitOfPooja = dto.BenefitOfPooja,
                PoojaDuration = dto.PoojaDuration,
                RequiredThings = dto.RequiredThings,
                IsActive = dto.IsActive,
                CreatedDate = DateTime.Now
            };

            _context.PujaTypes.Add(pujaType);
            await _context.SaveChangesAsync();

            return pujaType;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating puja type");
            throw;
        }
    }

    public async Task<PujaType?> UpdatePujaTypeAsync(int id, UpdatePujaTypeDto dto)
    {
        try
        {
            // Validate required fields
            ValidatePujaTypeDto(dto);

            var pujaType = await _context.PujaTypes.FindAsync(id);

            if (pujaType == null)
            {
                return null;
            }

            pujaType.PujaTypeName = dto.PujaTypeName.Trim();
            pujaType.Description = dto.Description;
            pujaType.Price = dto.Price;
            pujaType.ImageUrl = dto.ImageUrl;
            pujaType.BenefitOfPooja = dto.BenefitOfPooja;
            pujaType.PoojaDuration = dto.PoojaDuration;
            pujaType.RequiredThings = dto.RequiredThings;
            pujaType.IsActive = dto.IsActive;

            _context.Entry(pujaType).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return pujaType;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            if (!await PujaTypeExistsAsync(id))
            {
                _logger.LogWarning("Puja type with ID {Id} not found during update", id);
                return null;
            }
            _logger.LogError(ex, "Concurrency error updating puja type with ID {Id}", id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating puja type with ID {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeletePujaTypeAsync(int id)
    {
        try
        {
            var pujaType = await _context.PujaTypes.FindAsync(id);

            if (pujaType == null)
            {
                return false;
            }

            _context.PujaTypes.Remove(pujaType);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting puja type with ID {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeactivatePujaTypeAsync(int id)
    {
        try
        {
            var pujaType = await _context.PujaTypes.FindAsync(id);

            if (pujaType == null)
            {
                return false;
            }

            pujaType.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating puja type with ID {Id}", id);
            throw;
        }
    }

    public async Task<bool> PujaTypeExistsAsync(int id)
    {
        return await _context.PujaTypes.AnyAsync(e => e.PujaTypeId == id);
    }

    private void ValidatePujaTypeDto(dynamic dto)
    {
        var errors = new List<string>();

        // Validate PujaTypeName (required, max 500 characters)
        if (string.IsNullOrWhiteSpace(dto.PujaTypeName))
        {
            errors.Add("PujaTypeName is required and cannot be empty");
        }
        else if (dto.PujaTypeName.Trim().Length > 500)
        {
            errors.Add("PujaTypeName cannot exceed 500 characters");
        }

        // Validate Price (required, must be >= 0)
        if (dto.Price < 0)
        {
            errors.Add("Price must be greater than or equal to 0");
        }

        // Validate optional fields length
        if (!string.IsNullOrEmpty(dto.ImageUrl) && dto.ImageUrl.Length > 500)
        {
            errors.Add("ImageUrl cannot exceed 500 characters");
        }

        if (errors.Count > 0)
        {
            var errorMessage = string.Join("; ", errors);
            _logger.LogWarning("Validation failed: {Errors}", errorMessage);
            throw new ArgumentException(errorMessage);
        }
    }
}
