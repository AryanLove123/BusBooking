namespace BusBooking.Application.DTOs.Bus;

public class BusSearchResponse
{
    public int BusId { get; set; }
    public string OperatorName { get; set; } = string.Empty;
    public string BusNumber { get; set; } = string.Empty;
    public string BusType { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime DepartureUtc { get; set; }
    public DateTime ArrivalUtc { get; set; }
    public int AvailableSeats { get; set; }
    public decimal FarePerSeat { get; set; }
}