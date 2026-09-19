using System.ComponentModel.DataAnnotations;
using BusBooking.Application.DTOs;
using BusBooking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BusBooking.API.Controllers;

[ApiController]
[Route("api/auth")]

public class AuthController : ControllerBase
{
    private IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await _authService.RegisterAsync(request, ct);
        return CreatedAtAction(nameof(Register), new { id = result.UserId }, result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _authService.LoginAsync(request, ct);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message }); // Returns clean HTTP 400
        }
    }
}