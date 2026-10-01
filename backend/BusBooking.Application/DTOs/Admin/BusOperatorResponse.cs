namespace BusBooking.Application.DTOs.Admin;

public class BusOperatorResponse
{
    public int OperatorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public int BusCount { get; set; }
}