namespace BusBooking.Domain.Exceptions;

public class InsufficientSeatsException : Exception
{
    public InsufficientSeatsException(int requested, int available): base($"Requested {requested} seats but only {available} are available") {}
}