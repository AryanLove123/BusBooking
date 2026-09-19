namespace BusBooking.Domain.Exceptions;

public class DuplicateEmailException : Exception
{
    public DuplicateEmailException(string email) : base($"An account with email '{email}' already exists."){ }
}