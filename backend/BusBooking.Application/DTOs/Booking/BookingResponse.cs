namespace BusBooking.Application.DTOs.Booking;

public class BookingResponse
{
    public int BookingId { get; set; }
    public int BusId { get; set; }
    public string OperatorName { get; set; } = string.Empty;
    public string BusNumber { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime DepartureUtc { get; set; }
    public DateTime ArrivalUtc { get; set; }
    public int SeatsBooked { get; set; }
    public decimal TotalFare { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime BookingDateUtc { get; set; }
    public List<PassengerDto> Passengers { get; set; } = new();
}