namespace BusBooking.Application.Interfaces;

public interface IAppLogger
{
    Task LogInformationAsync(string message, int? userId = null, string? requestPath = null, string? traceId = null);
    Task LogWarningAsync(string message, int? userId = null, string? requestPath = null, string? traceId = null);
    Task LogErrorAsync(string message, Exception? exception, int? userId = null, string? requestPath = null, string? traceId = null);
}