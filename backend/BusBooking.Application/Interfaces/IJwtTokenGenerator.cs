using BusBooking.Domain.Models;

namespace BusBooking.Application.Interfaces;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAtUtc) GenerateToken(User user);
}