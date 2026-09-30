using BusBooking.Application.DTOs;
using BusBooking.Application.DTOs.Admin;

namespace BusBooking.Application.Interfaces;

public interface IAdminService
{
    Task<List<AdminBusResponse>> GetBusesAsync(CancellationToken ct = default);
    Task<List<AdminBookingResponse>> GetBookingsForBusAsync(int busId, CancellationToken ct = default);
    Task<AdminBusResponse> CreateBusAsync(CreateBusRequest request, CancellationToken ct = default);
}