using System.ComponentModel.DataAnnotations;
using BusBooking.Application.Common;
using BusBooking.Application.DTOs.Bus;
using BusBooking.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusBooking.Application.Services;

public class BusService : IBusService
{
    private IAppDbContext _db;

    public BusService(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<List<BusSearchResponse>> SearchAsync(BusSearchRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Source) || string.IsNullOrWhiteSpace(request.Destination))
        {
            throw new ValidationException("Source and Destination are required");
        }

        if (string.Equals(request.Source.Trim(), request.Destination.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            throw new ValidationException("Source and Destination cannot be identical");
        }

        var source = request.Source.Trim().ToLower();
        var destination = request.Destination.Trim().ToLower();

        var buses = await _db.Buses
            .Include(b => b.BusOperator)
            .Where(b => b.Source.ToLower() == source && b.Destination.ToLower() == destination)
            .ToListAsync(ct);

        var nowUtc = DateTime.UtcNow;

        var results = buses.Select(b => new BusSearchResponse
        {
            BusId = b.Id,
            OperatorName = b.BusOperator?.Name ?? "Unknown",
            BusNumber = b.BusNumber,
            BusType = b.BusType,
            Source = b.Source,
            Destination = b.Destination,
            DepartureUtc = b.GetNextDepartureUtc(nowUtc),
            ArrivalUtc = b.GetNextArrivalUtc(nowUtc),
            AvailableSeats = b.AvailableSeats,
            FarePerSeat = b.FarePerSeat
        }).AsEnumerable();

        if (request.PreferredDepartureAfterUTC.HasValue)
        {
            results = results.Where(b => b.DepartureUtc >= request.PreferredDepartureAfterUTC.Value);
        }

        if (request.PreferredArrivalBeforeUTC.HasValue)
        {
            results = results.Where(b => b.ArrivalUtc <= request.PreferredArrivalBeforeUTC.Value);
        }

        return results.OrderBy(b => b.DepartureUtc).ToList();
    }
}