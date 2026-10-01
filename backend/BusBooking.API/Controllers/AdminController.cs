using BusBooking.Application.DTOs;
using BusBooking.Application.DTOs.Admin;
using BusBooking.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusBooking.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]

public class AdminController : ControllerBase
{
    private IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("buses")]

    public async Task<ActionResult<List<AdminBusResponse>>> GetBuses(CancellationToken ct)
    {
        var result = await _adminService.GetBusesAsync(ct);
        return Ok(result);
    }

    [HttpGet("buses/{busId:int}/bookings")]
    public async Task<ActionResult<List<AdminBookingResponse>>> GetBookingsForBus(int busId, CancellationToken ct)
    {
        var result = await _adminService.GetBookingsForBusAsync(busId, ct);
        return Ok(result);
    }

    [HttpPost("buses")]
    public async Task<ActionResult<AdminBusResponse>> CreateBus([FromBody] CreateBusRequest request, CancellationToken ct)
    {
        var result = await _adminService.CreateBusAsync(request, ct);
        return CreatedAtAction(nameof(GetBuses), result);
    }

    [HttpGet("operators")]
    public async Task<ActionResult<List<BusOperatorResponse>>> GetBusOperators(CancellationToken ct)
    {
        var result = await _adminService.GetBusOperatorsAsync(ct);
        return Ok(result);
    }
}