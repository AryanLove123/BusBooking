using System.ComponentModel.DataAnnotations;
using BusBooking.Domain.Enums;

namespace BusBooking.Application.DTOs.Booking;

public class PassengerDto
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 120)]
    public int Age { get; set; }
    
    [Required]
    public Gender Gender { get; set; }
}