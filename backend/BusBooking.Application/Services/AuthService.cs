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
    private IAppLogger _logger;

    public AuthService(IAppDbContext db, IPasswordHasher hasher, IJwtTokenGenerator tokenGenerator, IAppLogger logger)
    {
        _db = db;
        _hasher = hasher;
        _tokenGenerator = tokenGenerator;
        _logger = logger;
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var exists = await _db.Users.AnyAsync(u => u.Email == normalizedEmail, ct);
        if (exists)
        {
            await _logger.LogWarningAsync($"Attempt to register with duplicate email: {normalizedEmail}");
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

        await _logger.LogInformationAsync($"User registration successful for {normalizedEmail}", user.Id);

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
            await _logger.LogWarningAsync($"Failed login attempt for {normalizedEmail}");
            throw new ValidationException("Invalid email or password");
        }

        await _logger.LogInformationAsync($"User login successful for {normalizedEmail}", user.Id);

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