using BusBooking.Application.DTOs.Booking;
using BusBooking.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusBooking.API.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize]

public class BookingsController : ControllerBase
{
    private IBookingService _bookingService;

    private ICurrentUserService _currentUserService;

    public BookingsController(IBookingService bookingService, ICurrentUserService currentUserService)
    {
        _bookingService = bookingService;
        _currentUserService = currentUserService;
    }

    private int UserId => _currentUserService.UserId ?? throw new UnauthorizedAccessException();

    [HttpPost]
    public async Task<ActionResult<BookingResponse>> Create([FromBody] CreateBookingRequest request, CancellationToken ct)
    {
        var result = await _bookingService.CreateBookingAsync(UserId, request, ct);
        return CreatedAtAction(nameof(GetBookingById), new {id = result.BookingId}, result);
    }

    [HttpGet("my")]
    public async Task<ActionResult<List<BookingResponse>>> GetMyBookings(CancellationToken ct)
    {
        var result = await _bookingService.GetMyBookingsAsync(UserId,ct);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingResponse>> GetBookingById(int id, CancellationToken ct)
    {
        var result = await _bookingService.GetBookingByIdAsync(UserId, id, ct);
        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<BookingResponse>> Update(int id, [FromBody] UpdateBookingRequest updatedRequest, CancellationToken ct)
    {
        var result = await _bookingService.UpdateBookingAsync(UserId, id, updatedRequest, ct);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<BookingResponse>> Cancel(int id, CancellationToken ct)
    {
        var result = await _bookingService.CancelBookingAsync(UserId,id,ct);
        return Ok(result);
    }
}