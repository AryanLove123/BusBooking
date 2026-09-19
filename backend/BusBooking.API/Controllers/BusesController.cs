using System.ComponentModel.DataAnnotations;
using BusBooking.Application.DTOs.Bus;
using BusBooking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BusBooking.API.Controllers;

[ApiController]
[Route("api/buses")]

public class BusesController : ControllerBase
{
    private IBusService _busService;
    public BusesController(IBusService busService)
    {
        _busService = busService;
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<BusSearchResponse>>> Search(
       [FromQuery] string source,
       [FromQuery] string destination,
       [FromQuery] DateTime? departureAfter,
       [FromQuery] DateTime? arrivalAfter,
       CancellationToken ct)
    {
        try
        {
            var request = new BusSearchRequest
            {
                Source = source,
                Destination = destination,
                PreferredArrivalBeforeUTC = arrivalAfter,
                PreferredDepartureAfterUTC = departureAfter
            };

            var results = await _busService.SearchAsync(request, ct);
            return Ok(results);

        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message }); // Returns clean HTTP 400
        }
    }
}