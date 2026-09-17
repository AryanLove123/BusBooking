using BusBooking.Application.Interfaces;

namespace BusBooking.Infrastructure.Utility;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string plainTextPassword) => BCrypt.Net.BCrypt.HashPassword(plainTextPassword, workFactor: 11);
    public bool Verify(string plainTextPassword, string hash) => BCrypt.Net.BCrypt.Verify(plainTextPassword, hash);
}