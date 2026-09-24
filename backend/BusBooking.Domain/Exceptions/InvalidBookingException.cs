namespace BusBooking.Domain.Exceptions;

public class InvalidBookingOperationException : Exception
{
    public InvalidBookingOperationException(string message) : base(message) { }
}