namespace BusBooking.Application.DTOs.Admin;

public class AdminBookingResponse
{
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public int SeatsBooked { get; set; }
    public decimal TotalFare { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime BookingDateUtc { get; set; }
}