using System.ComponentModel.DataAnnotations;

namespace BusBooking.Application.DTOs;

public class RegisterRequest
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } =  string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength =8)]
    public string Password { get; set; } = string.Empty;
}