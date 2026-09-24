using BusBooking.Application.DTOs.Booking;

namespace BusBooking.Application.Interfaces;

public interface IBookingService
{
    Task<BookingResponse> CreateBookingAsync(int userId, CreateBookingRequest request, CancellationToken ct = default);
    Task<List<BookingResponse>> GetMyBookingsAsync(int userId, CancellationToken ct = default);
    Task<BookingResponse> GetBookingByIdAsync(int userId, int bookingId, CancellationToken ct = default);
    Task<BookingResponse> UpdateBookingAsync(int userId, int bookingId, UpdateBookingRequest updatedRequest, CancellationToken ct = default);
    Task<BookingResponse> CancelBooking(int userId, int bookingId, CancellationToken ct = default);
}