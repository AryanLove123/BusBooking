using BusBooking.Domain.Enums;

namespace BusBooking.Domain.Models;

public class ApplicationLog
{
    public long Id { get; set; }
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    public AppLogLevel Level { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ExceptionMessage { get; set; }
    public string? StackTrace { get; set; }
    public string? Source { get; set; }
    public int? UserId { get; set; }
    public string? RequestPath { get; set; }
    public string? TraceId { get; set; }
}