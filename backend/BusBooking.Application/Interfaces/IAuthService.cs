using BusBooking.Application.DTOs;

namespace BusBooking.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
}