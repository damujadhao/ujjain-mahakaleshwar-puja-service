namespace poojaPathBooking.Services.Interfaces;

using poojaPathBooking.Models.DTOs;
using poojaPathBooking.Models.Entities;

public interface IPujaBookingService
{
    Task<IEnumerable<PujaBookingResponseDto>> GetAllBookingsAsync();
    Task<PujaBookingResponseDto?> GetBookingByIdAsync(int id);
    Task<IEnumerable<PujaBookingResponseDto>> GetBookingsByCustomerIdAsync(Guid customerId);
    Task<IEnumerable<PujaBookingResponseDto>> GetBookingsByPujaTypeIdAsync(int pujaTypeId);
    Task<IEnumerable<PujaBookingResponseDto>> GetBookingsByStatusAsync(string status);
    Task<IEnumerable<PujaBookingResponseDto>> GetBookingsByDateRangeAsync(DateOnly startDate, DateOnly endDate);
    Task<IEnumerable<PujaBookingResponseDto>> GetPendingPaymentsAsync();
    Task<PujaBooking> CreateBookingAsync(CreatePujaBookingDto dto);
    Task<PujaBooking?> UpdateBookingAsync(int id, UpdatePujaBookingDto dto);
    Task<bool> UpdateBookingStatusAsync(int id, UpdateBookingStatusDto dto);
    Task<bool> UpdatePaymentStatusAsync(int id, UpdatePaymentStatusDto dto);
    Task<bool> DeleteBookingAsync(int id);
    Task<bool> CancelBookingAsync(int id);
    Task<bool> BookingExistsAsync(int id);
    Task<decimal> GetTotalRevenueAsync();
    Task<int> GetTotalBookingsCountAsync();
}
