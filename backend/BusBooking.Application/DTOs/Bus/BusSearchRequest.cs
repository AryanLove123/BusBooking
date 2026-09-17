namespace BusBooking.Application.DTOs.Bus;

public class BusSearchRequest
{
    public string Source {get; set;} =  string.Empty;
    public string Destination {get; set;} = string.Empty;
    public DateTime? PreferredDepartureAfterUTC {get; set;}
    public DateTime? PreferredArrivalBeforeUTC {get; set;}
}