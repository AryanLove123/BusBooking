using BusBooking.Domain.Enums;

namespace BusBooking.Domain.Models;

public class Booking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int BusId { get; set; }
    public Bus? Bus { get; set; }
    public int SeatsBooked { get; set; }
    public decimal TotalFare { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Confirmed;
    public DateTime BookingDateUtc { get; set; } = DateTime.UtcNow;
    public ICollection<Passenger> Passengers { get; set; } = new List<Passenger>();
}
