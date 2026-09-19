using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BusBooking.Application.Interfaces;
using BusBooking.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BusBooking.Infrastructure.Utility;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private IConfiguration _config;

    public JwtTokenGenerator(IConfiguration config)
    {
        _config = config;
    }

    public (string Token, DateTime ExpiresAtUtc) GenerateToken(User user)
    {
        var jwtSection = _config.GetSection("Jwt");
        var secret = jwtSection["Secret"] ?? throw new InvalidOperationException("Jwt Secret is not configured.");
        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];
        var expiryMinutes = int.TryParse(jwtSection["ExpiryMinutes"], out var m) ? m : 120;    
        var expires = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token =  new JwtSecurityToken(
            issuer : issuer,
            audience : audience,
            claims : claims,
            expires : expires,
            signingCredentials: creds
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}