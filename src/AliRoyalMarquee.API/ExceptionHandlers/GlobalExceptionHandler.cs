using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Net;

namespace AliRoyalMarquee.API.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path
        };

        if (exception is ValidationException validationException)
        {
            problemDetails.Title = "Validation Error";
            problemDetails.Status = (int)HttpStatusCode.BadRequest;
            problemDetails.Detail = "One or more validation errors occurred.";
            problemDetails.Extensions["errors"] = validationException.Errors
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(g => g.Key, g => g.ToArray());
        }
        else if (exception is PostgresException postgresException && postgresException.SqlState == "23P01")
        {
            problemDetails.Title = "Booking conflict";
            problemDetails.Status = (int)HttpStatusCode.Conflict;
            problemDetails.Detail = "The selected venue is already booked for the requested time.";
        }
        else if (exception is InvalidOperationException)
        {
            problemDetails.Title = "Bad Request";
            problemDetails.Status = (int)HttpStatusCode.BadRequest;
            problemDetails.Detail = exception.Message;
        }
        else
        {
            problemDetails.Title = "Internal Server Error";
            problemDetails.Status = (int)HttpStatusCode.InternalServerError;
            problemDetails.Detail = "An unexpected error occurred.";
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
