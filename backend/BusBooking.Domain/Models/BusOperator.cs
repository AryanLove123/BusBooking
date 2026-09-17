namespace BusBooking.Domain.Models;

public class BusOperator
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public ICollection<Bus> Buses { get; set; } = new List<Bus>();
}
