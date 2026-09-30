using System.ComponentModel.DataAnnotations;

namespace BusBooking.Application.DTOs.Admin;

public class CreateBusRequest
{
    [Required]
    public int BusOperatorId { get; set; }

    [Required, StringLength(30)]
    public string BusNumber { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string BusType { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Source { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Destination { get; set; } = string.Empty;
    
    [Required]
    public string DepartureTime { get; set; } = string.Empty;

    [Range(1, 2880)]
    public int DurationMinutes { get; set; }

    [Range(1, 100)]
    public int TotalSeats { get; set; }

    [Range(10, 10000)]
    public decimal FarePerSeat { get; set; }
}