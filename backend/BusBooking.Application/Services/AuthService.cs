using System.ComponentModel.DataAnnotations;
using BusBooking.Application.Common;
using BusBooking.Application.DTOs;
using BusBooking.Application.Interfaces;
using BusBooking.Domain.Enums;
using BusBooking.Domain.Exceptions;
using BusBooking.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BusBooking.Application.Services;

public class AuthService : IAuthService
{
    private IAppDbContext _db;
    private IPasswordHasher _hasher;
    private IJwtTokenGenerator _tokenGenerator;

    public AuthService(IAppDbContext db, IPasswordHasher hasher, IJwtTokenGenerator tokenGenerator)
    {
        _db = db;
        _hasher = hasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var exists = await _db.Users.AnyAsync(u => u.Email == normalizedEmail, ct);
        if (exists)
        {
            throw new DuplicateEmailException(normalizedEmail);
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = normalizedEmail,
            PasswordHash = _hasher.Hash(request.Password),
            Role = UserRole.User
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        var (token, expires) = _tokenGenerator.GenerateToken(user);

        return new LoginResponse
        {
            Token = token,
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
            ExpiresAtUtc = expires
        };
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail, ct);

        if (user is null || !_hasher.Verify(request.Password, user.PasswordHash))
        {
            throw new ValidationException("Invalid email or password");
        }

        var (token, expires) = _tokenGenerator.GenerateToken(user);

        return new LoginResponse
        {
            Token = token,
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
            ExpiresAtUtc = expires
        };
    }
}