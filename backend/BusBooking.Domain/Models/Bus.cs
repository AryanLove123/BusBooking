namespace BusBooking.Domain.Models;

public class Bus
{
    public int Id { get; set; }
    public int BusOperatorId { get; set; }
    public BusOperator? BusOperator { get; set; }
    public string BusNumber { get; set; } = string.Empty;
    public string BusType { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public TimeSpan DepartureTime { get; set; }
    public int DurationMinutes { get; set; }
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public decimal FarePerSeat { get; set; }

    [System.ComponentModel.DataAnnotations.Timestamp]
    public byte[]? RowVersion { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public DateTime GetNextDepartureUtc(DateTime nowUtc)
    {
        var todaysDeparture = nowUtc.Date + DepartureTime;
        return todaysDeparture > nowUtc ? todaysDeparture : todaysDeparture.AddDays(1);
    }
    public DateTime GetNextArrivalUtc(DateTime nowUtc) => GetNextDepartureUtc(nowUtc).AddMinutes(DurationMinutes);
}
