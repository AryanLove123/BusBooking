using System.ComponentModel.DataAnnotations;
using System.Net;
using BusBooking.Domain.Exceptions;
using ValidationException = BusBooking.Domain.Exceptions.ValidationException;

namespace BusBooking.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var traceId = context.TraceIdentifier;
            var (statusCode, message) = MapException(ex);

            int? userId = null;
            var userIdClaim = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out var parsedUserId)) userId = parsedUserId;

            if(statusCode == HttpStatusCode.InternalServerError)
            {
                _logger.LogError(ex, "Unhandled exception occurred. TraceId: {TraceId}, UserId: {UserId}", traceId, userId);
            }
            else
            {
                _logger.LogWarning(ex, "Handled exception occurred. TraceId: {TraceId}, UserId: {UserId}", traceId, userId);
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            object body = ex is ValidationException ve
                ? new { statusCode = (int)statusCode, message = ve.Message, errors = ve.Errors, traceId }
                : new { statusCode = (int)statusCode, message, traceId };

            await context.Response.WriteAsJsonAsync(body);

        }
    }

    private static (HttpStatusCode StatusCode, string Message) MapException(Exception ex) => ex switch
    {
        ValidationException => (HttpStatusCode.BadRequest, ex.Message),
        UnauthorizedAccessException => (HttpStatusCode.Unauthorized, ex.Message),
        EntityNotFoundException => (HttpStatusCode.NotFound, ex.Message),
        InsufficientSeatsException => (HttpStatusCode.BadRequest, ex.Message),
        DuplicateEmailException => (HttpStatusCode.BadRequest, ex.Message),
        InvalidBookingOperationException => (HttpStatusCode.BadRequest, ex.Message),
        _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
    };
}