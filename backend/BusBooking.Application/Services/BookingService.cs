using BusBooking.Application.Common;
using BusBooking.Application.DTOs.Booking;
using BusBooking.Application.Interfaces;
using BusBooking.Domain.Enums;
using BusBooking.Domain.Exceptions;
using BusBooking.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BusBooking.Application.Services;

public class BookingService : IBookingService
{
    private IAppDbContext _db;

    private const int maxConcurrencyRetries = 3;

    public BookingService(IAppDbContext db)
    {
        _db = db;
    }

    private static void ValidatePassengerCnt(int seatsRequested, List<PassengerDto> passengers)
    {
        if (passengers.Count != seatsRequested)
        {
            throw new ValidationException($"Number of passengers {passengers.Count} should match the seats requested {seatsRequested}.");
        }
    }

    private async Task<BookingResponse> MapToResponseAsync(int bookingId, CancellationToken ct)
    {
        var booking = await _db.Bookings.Include(b => b.Bus)
                        .ThenInclude(bus => bus!.BusOperator)
                        .Include(b => b.Passengers)
                        .FirstAsync(b => b.Id == bookingId, ct);
        return new BookingResponse
        {
            BookingId = booking.Id,
            BusId = booking.BusId,
            OperatorName = booking.Bus!.BusOperator!.Name,
            BusNumber = booking.Bus.BusNumber,
            Source = booking.Bus.Source,
            Destination = booking.Bus.Destination,
            DepartureUtc = booking.DepartureUtc,
            ArrivalUtc = booking.ArrivalUtc,
            SeatsBooked = booking.SeatsBooked,
            TotalFare = booking.TotalFare,
            Status = booking.Status.ToString(),
            BookingDateUtc = booking.BookingDateUtc,
            Passengers = booking.Passengers.Select(p => new PassengerDto
            {
                Name = p.Name,
                Age = p.Age,
                Gender = p.Gender
            }).ToList()
        };
    }
    public async Task<BookingResponse> CreateBookingAsync(int userId, CreateBookingRequest request, CancellationToken ct = default)
    {
        ValidatePassengerCnt(request.SeatsRequested, request.Passengers);

        for (var attempt = 1; attempt <= maxConcurrencyRetries; attempt++)
        {
            await using var transaction = await _db.BeginTransactionAsync(ct);
            try
            {
                _db.ChangeTracker.Clear();
                var bus = await _db.Buses.FirstOrDefaultAsync(b => b.Id == request.BusId, ct) ?? throw new EntityNotFoundException("Bus", request.BusId);

                if (bus.AvailableSeats < request.SeatsRequested)
                {
                    throw new InsufficientSeatsException(request.SeatsRequested, bus.AvailableSeats);
                }

                bus.AvailableSeats -= request.SeatsRequested;

                var nowUtc = DateTime.UtcNow;
                var booking = new Booking
                {
                    UserId = userId,
                    BusId = bus.Id,
                    SeatsBooked = request.SeatsRequested,
                    TotalFare = bus.FarePerSeat * request.SeatsRequested,
                    Status = BookingStatus.Confirmed,
                    DepartureUtc = bus.GetNextDepartureUtc(nowUtc),
                    ArrivalUtc = bus.GetNextArrivalUtc(nowUtc),
                    BookingDateUtc = nowUtc,
                    Passengers = request.Passengers.Select(p => new Passenger
                    {
                        Name = p.Name.Trim(),
                        Age = p.Age,
                        Gender = p.Gender
                    }).ToList()
                };

                _db.Bookings.Add(booking);

                await _db.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                return await MapToResponseAsync(booking.Id, ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (attempt == maxConcurrencyRetries)
                {
                    throw new InvalidBookingOperationException("Could not complete the booking due to high demand for this bus. Please try again.");
                }
                await Task.Delay(50 * attempt, ct);
            }
        }
        throw new InvalidBookingOperationException("Could not complete the booking due to high demand for this bus. Please try again.");
    }

    public async Task<BookingResponse> GetBookingByIdAsync(int userId, int bookingId, CancellationToken ct = default)
    {
        var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId) ?? throw new EntityNotFoundException("Booking", bookingId);

        if (userId != booking.UserId)
        {
            throw new UnauthorizedAccessException("You are not allowed to access or modify this booking");
        }
        return await MapToResponseAsync(bookingId, ct);
    }

    public async Task<List<BookingResponse>> GetMyBookingsAsync(int userId, CancellationToken ct = default)
    {
        var bookingIds = await _db.Bookings.Where(b => b.UserId == userId)
                            .OrderByDescending(b => b.BookingDateUtc)
                            .Select(b => b.Id)
                            .ToListAsync(ct);

        var results = new List<BookingResponse>();
        foreach (var id in bookingIds)
        {
            results.Add(await MapToResponseAsync(id, ct));
        }
        return results;
    }

    public async Task<BookingResponse> UpdateBookingAsync(int userId, int bookingId, UpdateBookingRequest updatedRequest, CancellationToken ct = default)
    {
        ValidatePassengerCnt(updatedRequest.SeatsRequested, updatedRequest.Passengers);

        for (var attempt = 1; attempt <= maxConcurrencyRetries; attempt++)
        {
            await using var transaction = await _db.BeginTransactionAsync(ct);
            try
            {
                _db.ChangeTracker.Clear();

                var booking = await _db.Bookings.Include(b => b.Passengers)
                                .FirstOrDefaultAsync(b => b.Id == bookingId, ct) ?? throw new EntityNotFoundException("Booking", bookingId);

                if (booking.UserId != userId)
                {
                    throw new UnauthorizedAccessException("You are not allowed to access or modify this booking");

                }

                if (booking.Status == BookingStatus.Cancelled)
                {
                    throw new InvalidBookingOperationException("A cancelled booking cannot be edited");
                }

                var bus = await _db.Buses.FirstOrDefaultAsync(bus => bus.Id == booking.BusId, ct) ?? throw new EntityNotFoundException("Bus", booking.BusId);

                if (booking.DepartureUtc <= DateTime.UtcNow)
                {
                    throw new InvalidBookingOperationException("This bus has already departed; the booking can no longer be edited.");
                }

                var seatDelta = updatedRequest.SeatsRequested - booking.SeatsBooked;

                if (seatDelta > 0 && bus.AvailableSeats < seatDelta)
                {
                    throw new InsufficientSeatsException(seatDelta, bus.AvailableSeats);
                }

                bus.AvailableSeats -= seatDelta;

                booking.SeatsBooked = updatedRequest.SeatsRequested;
                booking.TotalFare = bus.FarePerSeat * updatedRequest.SeatsRequested;

                _db.Passengers.RemoveRange(booking.Passengers);

                booking.Passengers = updatedRequest.Passengers.Select(p => new Passenger
                {
                    Name = p.Name.Trim(),
                    Age = p.Age,
                    Gender = p.Gender,
                    BookingId = booking.Id
                }).ToList();

                await _db.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                return await MapToResponseAsync(bookingId, ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (attempt == maxConcurrencyRetries)
                {
                    throw new InvalidBookingOperationException("Could not complete the booking due to high demand for this bus. Please try again.");
                }
                await Task.Delay(50 * attempt, ct);
            }
        }
        throw new InvalidBookingOperationException("Could not update the booking due to a concurrent seat change. Please try again.");
    }

    public async Task<BookingResponse> CancelBookingAsync(int userId, int bookingId, CancellationToken ct = default)
    {
        for (var attempt = 1; attempt <= maxConcurrencyRetries; attempt++)
        {
            await using var transaction = await _db.BeginTransactionAsync(ct);
            try
            {
                _db.ChangeTracker.Clear();
                var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId, ct) ?? throw new EntityNotFoundException("Booking", bookingId);

                if (booking.UserId != userId)
                {
                    throw new UnauthorizedAccessException("You are not allowed to access or modify this booking");
                }

                if (booking.Status == BookingStatus.Cancelled)
                {
                    throw new InvalidBookingOperationException("This booking is already cancelled");
                }

                var bus = await _db.Buses.FirstOrDefaultAsync(b => b.Id != booking.BusId, ct) ?? throw new EntityNotFoundException("Bus", booking.BusId);

                bus.AvailableSeats += booking.SeatsBooked;
                booking.Status = BookingStatus.Cancelled;

                await _db.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                return await MapToResponseAsync(bookingId, ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (attempt == maxConcurrencyRetries)
                {
                    throw new InvalidBookingOperationException("Could not complete the booking due to high demand for this bus. Please try again.");
                }
                await Task.Delay(50 * attempt, ct);
            }
        }
        throw new InvalidBookingOperationException("Could not cancel the booking due to a concurrent update. Please try again.");
    }
}