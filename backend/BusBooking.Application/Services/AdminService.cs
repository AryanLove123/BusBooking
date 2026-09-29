using BusBooking.Application.Common;
using BusBooking.Application.DTOs;
using BusBooking.Application.DTOs.Admin;
using BusBooking.Application.Interfaces;
using BusBooking.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BusBooking.Application.Services;

public class AdminService : IAdminService
{
    private IAppDbContext _db;

    public AdminService(IAppDbContext db)
    {
        _db = db;
    }
    public async Task<List<AdminBookingResponse>> GetBookingsForBusAsync(int busId, CancellationToken ct = default)
    {
        var busExist = await _db.Buses.FirstOrDefaultAsync(b => b.Id == busId) ?? throw new EntityNotFoundException("Bus", busId);

        return await _db.Bookings
        .Include(b => b.User)
        .Where(b => b.BusId == busId)
        .OrderByDescending(b => b.BookingDateUtc)
        .Select(b => new AdminBookingResponse
        {
            BookingId = b.Id,
            UserId = b.UserId,
            UserName = b.User!.Name,
            UserEmail = b.User.Email,
            SeatsBooked = b.SeatsBooked,
            TotalFare = b.TotalFare,
            Status = b.Status.ToString(),
            BookingDateUtc = b.BookingDateUtc
        }).ToListAsync(ct);

    }

    public async Task<List<AdminBusResponse>> GetBusesAsync(CancellationToken ct = default)
    {
        var buses = await _db.Buses.ToListAsync(ct);
        var nowUtc = DateTime.UtcNow;

        return buses
            .Select(b => new AdminBusResponse
            {
                BusId = b.Id,
                BusNumber = b.BusNumber,
                BusType = b.BusType,
                Source = b.Source,
                Destination = b.Destination,
                DepartureUtc = b.GetNextDepartureUtc(nowUtc),
                ArrivalUtc = b.GetNextArrivalUtc(nowUtc),
                TotalSeats = b.TotalSeats,
                AvailableSeats = b.AvailableSeats,
                FarePerSeat = b.FarePerSeat
            })
            .OrderBy(r => r.DepartureUtc)
            .ToList();
    }
}