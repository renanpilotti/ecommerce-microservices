using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Authentication;
using System.Security;
using System.Text.Json;

namespace BuildingBlocks.Exceptions
{
    public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var exceptionMessage = exception.Message;

            var (detail, title, statusCode) = GetExceptionDetails(exception);

            logger.LogWarning(exception,
                "Error: {ExceptionType} - {Message} at {Time}",
                exception.GetType().Name, exceptionMessage, DateTime.UtcNow);

            // Set the response status code
            httpContext.Response.StatusCode = statusCode;

            var problemDetails = new ProblemDetails
            {
                Title = title,
                Detail = detail,
                Status = statusCode,
                Instance = httpContext.Request.Path
            };

            // Add additional context for validation errors
            if (exception is FluentValidation.ValidationException validationException)
            {
                problemDetails.Extensions["errors"] = validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );
            }

            // Add trace ID for debugging
            if (httpContext.TraceIdentifier != null)
            {
                problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
            }

            httpContext.Response.ContentType = "application/json";

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            await httpContext.Response.WriteAsync(
                JsonSerializer.Serialize(problemDetails, jsonOptions),
                cancellationToken);

            return true;
        }

        private static (string Detail, string Title, int StatusCode) GetExceptionDetails(Exception exception)
        {
            return exception switch
            {
                // 400 Bad Request
                FluentValidation.ValidationException =>
                    (exception.Message, "Validation Error", StatusCodes.Status400BadRequest),

                BadRequestException =>
                    (exception.Message, "Bad Request", StatusCodes.Status400BadRequest),

                ArgumentException =>
                    (exception.Message, "Invalid Argument", StatusCodes.Status400BadRequest),

                System.ComponentModel.DataAnnotations.ValidationException =>
                    (exception.Message, "Validation Error", StatusCodes.Status400BadRequest),

                // 401 Unauthorized
                UnauthorizedAccessException =>
                    ("Access denied. Authentication required.", "Unauthorized", StatusCodes.Status401Unauthorized),

                AuthenticationException =>
                    ("Authentication failed.", "Unauthorized", StatusCodes.Status401Unauthorized),

                // 403 Forbidden
                ForbiddenException =>
                    (exception.Message, "Forbidden", StatusCodes.Status403Forbidden),

                SecurityException =>
                    ("Access denied. Insufficient permissions.", "Forbidden", StatusCodes.Status403Forbidden),

                // 404 Not Found
                NotFoundException =>
                    (exception.Message, "Resource Not Found", StatusCodes.Status404NotFound),

                FileNotFoundException =>
                    ("The requested file was not found.", "File Not Found", StatusCodes.Status404NotFound),

                DirectoryNotFoundException =>
                    ("The requested directory was not found.", "Directory Not Found", StatusCodes.Status404NotFound),

                // 500 Internal Server Error
                NotImplementedException =>
                    ("This feature is not yet implemented.", "Not Implemented", StatusCodes.Status501NotImplemented),

                TimeoutException =>
                    ("The operation timed out.", "Service Unavailable", StatusCodes.Status503ServiceUnavailable),

                _ => ("An unexpected error occurred.", "Internal Server Error", StatusCodes.Status500InternalServerError)
            };
        }
    }
}