using BusBooking.Application.DTOs.Bus;

namespace BusBooking.Application.Interfaces;

public interface IBusService
{
    Task<List<BusSearchResponse>> SearchAsync(BusSearchRequest request, CancellationToken ct = default);
}