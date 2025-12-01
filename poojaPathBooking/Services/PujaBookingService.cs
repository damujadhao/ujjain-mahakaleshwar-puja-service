namespace poojaPathBooking.Services;

using Microsoft.EntityFrameworkCore;
using poojaPathBooking.Data;
using poojaPathBooking.Models.DTOs;
using poojaPathBooking.Models.Entities;
using poojaPathBooking.Services.Interfaces;

public class PujaBookingService(ApplicationDbContext context, ILogger<PujaBookingService> logger) : IPujaBookingService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<PujaBookingService> _logger = logger;

    public async Task<IEnumerable<PujaBookingResponseDto>> GetAllBookingsAsync()
    {
        try
        {
            return await _context.Set<PujaBooking>()
                .Include(b => b.PujaType)
                .Include(b => b.Customer)
                .Select(b => MapToResponseDto(b))
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all bookings");
            throw;
        }
    }

    public async Task<PujaBookingResponseDto?> GetBookingByIdAsync(int id)
    {
        try
        {
            var booking = await _context.Set<PujaBooking>()
                .Include(b => b.PujaType)
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            return booking == null ? null : MapToResponseDto(booking);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking with ID {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<PujaBookingResponseDto>> GetBookingsByCustomerIdAsync(Guid customerId)
    {
        try
        {
            return await _context.Set<PujaBooking>()
                .Include(b => b.PujaType)
                .Include(b => b.Customer)
                .Where(b => b.CustomerId == customerId)
                .Select(b => MapToResponseDto(b))
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings for customer {CustomerId}", customerId);
            throw;
        }
    }

    public async Task<IEnumerable<PujaBookingResponseDto>> GetBookingsByPujaTypeIdAsync(int pujaTypeId)
    {
        try
        {
            return await _context.Set<PujaBooking>()
                .Include(b => b.PujaType)
                .Include(b => b.Customer)
                .Where(b => b.PujaTypeId == pujaTypeId)
                .Select(b => MapToResponseDto(b))
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings for puja type {PujaTypeId}", pujaTypeId);
            throw;
        }
    }

    public async Task<IEnumerable<PujaBookingResponseDto>> GetBookingsByStatusAsync(string status)
    {
        try
        {
            return await _context.Set<PujaBooking>()
                .Include(b => b.PujaType)
                .Include(b => b.Customer)
                .Where(b => b.BookingStatus == status)
                .Select(b => MapToResponseDto(b))
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings with status {Status}", status);
            throw;
        }
    }

    public async Task<IEnumerable<PujaBookingResponseDto>> GetBookingsByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        try
        {
            return await _context.Set<PujaBooking>()
                .Include(b => b.PujaType)
                .Include(b => b.Customer)
                .Where(b => b.PujaDate >= startDate && b.PujaDate <= endDate)
                .Select(b => MapToResponseDto(b))
                .OrderBy(b => b.PujaDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings between {StartDate} and {EndDate}", startDate, endDate);
            throw;
        }
    }

    public async Task<IEnumerable<PujaBookingResponseDto>> GetPendingPaymentsAsync()
    {
        try
        {
            return await _context.Set<PujaBooking>()
                .Include(b => b.PujaType)
                .Include(b => b.Customer)
                .Where(b => !b.IsPaid && b.BookingStatus != "Cancelled")
                .Select(b => MapToResponseDto(b))
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending payments");
            throw;
        }
    }

    public async Task<PujaBooking> CreateBookingAsync(CreatePujaBookingDto dto)
    {
        try
        {
            // Validate that PujaType exists
            var pujaType = await _context.PujaTypes.FindAsync(dto.PujaTypeId);
            if (pujaType == null)
            {
                throw new ArgumentException($"PujaType with ID {dto.PujaTypeId} not found");
            }

            // Validate that Customer exists
            var customer = await _context.Customers.FindAsync(dto.CustomerId);
            if (customer == null)
            {
                throw new ArgumentException($"Customer with ID {dto.CustomerId} not found");
            }

            // Validate booking date is not in the past
            if (dto.PujaDate < DateOnly.FromDateTime(DateTime.Now))
            {
                throw new ArgumentException("Booking date cannot be in the past");
            }

            var booking = new PujaBooking
            {
                PujaTypeId = dto.PujaTypeId,
                CustomerId = dto.CustomerId,
                BookingMode = dto.BookingMode.Trim(),
                PujaDate = dto.PujaDate,
                PujaTime = dto.PujaTime,
                PeopleCount = dto.PeopleCount,
                Note = dto.Note,
                BookingStatus = "Pending",
                TotalAmount = dto.TotalAmount,
                Currency = dto.Currency,
                IsPaid = false,
                CreatedDate = DateTime.Now
            };

            _context.Set<PujaBooking>().Add(booking);
            await _context.SaveChangesAsync();

            return booking;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            throw;
        }
    }

    public async Task<PujaBooking?> UpdateBookingAsync(int id, UpdatePujaBookingDto dto)
    {
        try
        {
            var booking = await _context.Set<PujaBooking>().FindAsync(id);

            if (booking == null)
            {
                return null;
            }

            // Validate that PujaType exists
            var pujaType = await _context.PujaTypes.FindAsync(dto.PujaTypeId);
            if (pujaType == null)
            {
                throw new ArgumentException($"PujaType with ID {dto.PujaTypeId} not found");
            }

            // Prevent updating cancelled bookings
            if (booking.BookingStatus == "Cancelled")
            {
                throw new InvalidOperationException("Cannot update a cancelled booking");
            }

            booking.PujaTypeId = dto.PujaTypeId;
            booking.BookingMode = dto.BookingMode.Trim();
            booking.PujaDate = dto.PujaDate;
            booking.PujaTime = dto.PujaTime;
            booking.PeopleCount = dto.PeopleCount;
            booking.Note = dto.Note;
            booking.BookingStatus = dto.BookingStatus;
            booking.TotalAmount = dto.TotalAmount;
            booking.Currency = dto.Currency;
            booking.IsPaid = dto.IsPaid;
            booking.UpdatedDate = DateTime.Now;

            _context.Entry(booking).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return booking;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            if (!await BookingExistsAsync(id))
            {
                _logger.LogWarning("Booking with ID {Id} not found during update", id);
                return null;
            }
            _logger.LogError(ex, "Concurrency error updating booking with ID {Id}", id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking with ID {Id}", id);
            throw;
        }
    }

    public async Task<bool> UpdateBookingStatusAsync(int id, UpdateBookingStatusDto dto)
    {
        try
        {
            var booking = await _context.Set<PujaBooking>().FindAsync(id);

            if (booking == null)
            {
                return false;
            }

            booking.BookingStatus = dto.BookingStatus;
            booking.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking status for ID {Id}", id);
            throw;
        }
    }

    public async Task<bool> UpdatePaymentStatusAsync(int id, UpdatePaymentStatusDto dto)
    {
        try
        {
            var booking = await _context.Set<PujaBooking>().FindAsync(id);

            if (booking == null)
            {
                return false;
            }

            booking.IsPaid = dto.IsPaid;
            booking.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment status for ID {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteBookingAsync(int id)
    {
        try
        {
            var booking = await _context.Set<PujaBooking>().FindAsync(id);

            if (booking == null)
            {
                return false;
            }

            _context.Set<PujaBooking>().Remove(booking);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking with ID {Id}", id);
            throw;
        }
    }

    public async Task<bool> CancelBookingAsync(int id)
    {
        try
        {
            var booking = await _context.Set<PujaBooking>().FindAsync(id);

            if (booking == null)
            {
                return false;
            }

            if (booking.BookingStatus == "Completed")
            {
                throw new InvalidOperationException("Cannot cancel a completed booking");
            }

            booking.BookingStatus = "Cancelled";
            booking.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking with ID {Id}", id);
            throw;
        }
    }

    public async Task<bool> BookingExistsAsync(int id)
    {
        return await _context.Set<PujaBooking>().AnyAsync(b => b.BookingId == id);
    }

    public async Task<decimal> GetTotalRevenueAsync()
    {
        try
        {
            return await _context.Set<PujaBooking>()
                .Where(b => b.IsPaid && b.BookingStatus != "Cancelled")
                .SumAsync(b => b.TotalAmount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating total revenue");
            throw;
        }
    }

    public async Task<int> GetTotalBookingsCountAsync()
    {
        try
        {
            return await _context.Set<PujaBooking>()
                .CountAsync(b => b.BookingStatus != "Cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total bookings count");
            throw;
        }
    }

    private static PujaBookingResponseDto MapToResponseDto(PujaBooking booking)
    {
        return new PujaBookingResponseDto
        {
            BookingId = booking.BookingId,
            PujaTypeId = booking.PujaTypeId,
            PujaTypeName = booking.PujaType?.PujaTypeName,
            CustomerId = booking.CustomerId,
            CustomerName = booking.Customer != null 
                ? $"{booking.Customer.FirstName} {booking.Customer.LastName}" 
                : null,
            CustomerEmail = booking.Customer?.Email,
            CustomerContact = booking.Customer?.ContactNumber,
            BookingMode = booking.BookingMode,
            PujaDate = booking.PujaDate,
            PujaTime = booking.PujaTime,
            PeopleCount = booking.PeopleCount,
            Note = booking.Note,
            BookingStatus = booking.BookingStatus,
            TotalAmount = booking.TotalAmount,
            Currency = booking.Currency,
            IsPaid = booking.IsPaid,
            CreatedDate = booking.CreatedDate,
            UpdatedDate = booking.UpdatedDate
        };
    }
}
