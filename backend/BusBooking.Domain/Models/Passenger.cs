using BusBooking.Domain.Enums;

namespace BusBooking.Domain.Models;

public class Passenger
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public Booking? Booking { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public Gender Gender { get; set; }
}
