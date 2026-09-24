namespace BusBooking.Domain.Exceptions;

public class InsufficeintSeatsException : Exception
{
    public InsufficeintSeatsException(int requested, int available): base($"Requested {requested} seats but only {available} are available") {}
}