using System.ComponentModel.DataAnnotations;
using System.Globalization;
using BusBooking.Application.Common;
using BusBooking.Application.DTOs;
using BusBooking.Application.DTOs.Admin;
using BusBooking.Application.Interfaces;
using BusBooking.Domain.Exceptions;
using BusBooking.Domain.Models;
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
        var busExist = await _db.Buses.AnyAsync(b => b.Id == busId);

        if (!busExist)
        {
            throw new EntityNotFoundException("Bus", busId);
        }

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

    public async Task<AdminBusResponse> CreateBusAsync(CreateBusRequest request, CancellationToken ct = default)
    {
        if(string.Equals(request.Source.Trim(), request.Destination.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            throw new ValidationException("Source and Destination cannot be identical");
        }

        var operatorExists = await _db.BusOperators.AnyAsync(o => o.Id == request.BusOperatorId, ct);

        if (!operatorExists)
        {
            throw new EntityNotFoundException("Bus Operator", request.BusOperatorId);
        }

        if (!TimeSpan.TryParseExact(request.DepartureTime, @"hh\:mm", CultureInfo.InvariantCulture, out var departureTime))
        {
            throw new ValidationException("Departure time must be in HH:mm 24-hour format, e.g. 22:00.");
        }

        var bus = new Bus
        {
            BusOperatorId = request.BusOperatorId,
            BusNumber = request.BusNumber,
            BusType = request.BusType.Trim(),
            Source = request.Source.Trim(),
            Destination = request.Destination.Trim(),
            DepartureTime = departureTime,
            DurationMinutes = request.DurationMinutes,
            TotalSeats = request.TotalSeats,
            AvailableSeats = request.TotalSeats,
            FarePerSeat = request.FarePerSeat
        };

        _db.Buses.Add(bus);
        await _db.SaveChangesAsync(ct);

        var nowUtc = DateTime.UtcNow;
        return new AdminBusResponse
        {
            BusId = bus.Id,
            BusNumber = bus.BusNumber,
            BusType = bus.BusType,
            Source = bus.Source,
            Destination = bus.Destination,
            DepartureUtc = bus.GetNextDepartureUtc(nowUtc),
            ArrivalUtc = bus.GetNextArrivalUtc(nowUtc),
            TotalSeats = bus.TotalSeats,
            AvailableSeats = bus.AvailableSeats,
            FarePerSeat = bus.FarePerSeat
        };
    }

    public async Task<List<BusOperatorResponse>> GetBusOperatorsAsync(CancellationToken ct = default)
    {
        return await _db.BusOperators
            .Select(o => new BusOperatorResponse
            {
                OperatorId = o.Id,
                Name = o.Name,
                ContactEmail = o.ContactEmail,
                ContactPhone = o.ContactPhone,
                BusCount = o.Buses.Count
            })
            .ToListAsync(ct);
    }
}