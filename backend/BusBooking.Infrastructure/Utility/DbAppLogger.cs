using BusBooking.Application.Interfaces;
using BusBooking.Domain.Enums;
using BusBooking.Domain.Models;
using BusBooking.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BusBooking.Infrastructure.Utility;

public class DbAppLogger : IAppLogger
{
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly ILogger<DbAppLogger> _fallbackLogger;

    public DbAppLogger(IServiceScopeFactory scopeFactory, ILogger<DbAppLogger> fallbackLogger)
    {
        _scopeFactory = scopeFactory;
        _fallbackLogger = fallbackLogger;
    }

    public Task LogInformationAsync(string message, int? userId = null, string? requestPath = null, string? traceId = null)
        => WriteAsync(AppLogLevel.Information, message, null, userId, requestPath, traceId);

    public Task LogWarningAsync(string message, int? userId = null, string? requestPath = null, string? traceId = null)
        => WriteAsync(AppLogLevel.Warning, message, null, userId, requestPath, traceId);

    public Task LogErrorAsync(string message, Exception? exception, int? userId = null, string? requestPath = null, string? traceId = null)
        => WriteAsync(AppLogLevel.Error, message, exception, userId, requestPath, traceId);

    private async Task WriteAsync(AppLogLevel level, string message, Exception? exception, int? userId, string? requestPath, string? traceId)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbContext.ApplicationLogs.Add(new ApplicationLog
            {
                TimestampUtc = DateTime.UtcNow,
                Level = level,
                Message = Truncate(message, 1000),
                ExceptionMessage = exception is null ? null : Truncate(exception.Message, 2000),
                StackTrace = exception?.StackTrace,
                Source = exception?.Source,
                UserId = userId,
                RequestPath = requestPath,
                TraceId = traceId
            });

            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _fallbackLogger.LogError(ex, "Failed to log to database. Original message: {Message}, TraceId: {TraceId}, UserId: {UserId}", message, traceId, userId);
        }
    }

    private static string Truncate(string value, int maxLength) =>
        value.Length <= maxLength ? value : value[..maxLength];
}