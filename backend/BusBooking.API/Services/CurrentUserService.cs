using System.Security.Claims;
using BusBooking.Application.Interfaces;

namespace BusBooking.API.Services;

public class CurrentUserService: ICurrentUserService
{
    private IHttpContextAccessor _accessor;

    public CurrentUserService(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public int? UserId
    {
        get
        {
            var value = _accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(value, out var id)? id : null;
        }
    }

    public string? Email => _accessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

    public bool IsAdmin => _accessor.HttpContext?.User?.IsInRole("Admin") ?? false;

}