using System.ComponentModel.DataAnnotations;

namespace BusBooking.Application.DTOs.Booking;

public class CreateBookingRequest
{
    [Required]
    public int BusId {get; set;}

    [Range(1,10)]
    public int SeatsRequested {get; set;}

    [Required, MinLength(1)]
    public List<PassengerDto> Passengers {get; set;} = new();

}