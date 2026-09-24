namespace BusBooking.Domain.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string entityName, object entityId)
        : base($"{entityName} with id {entityId} was not found.")
    {
    }
}