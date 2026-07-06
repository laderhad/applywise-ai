using ApplyWise.Api.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ApplyWise.Api.Infrastructure;

public sealed class ApiExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ApiExceptionHandler> _logger;

    public ApiExceptionHandler(ILogger<ApiExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problem = GetProblem(exception);

        if (problem is null)
        {
            return false;
        }

        _logger.LogError(
            exception,
            "Request failed with status code {StatusCode}.",
            problem.Value.StatusCode);

        await Results.Problem(
                statusCode: problem.Value.StatusCode,
                title: problem.Value.Title,
                detail: problem.Value.Detail)
            .ExecuteAsync(httpContext);

        return true;
    }

    private static (
        int StatusCode,
        string Title,
        string Detail)? GetProblem(Exception exception)
    {
        if (exception is OllamaServiceException)
        {
            return (
                StatusCodes.Status502BadGateway,
                "Ollama analysis failed",
                exception.Message);
        }

        if (IsDatabaseException(exception))
        {
            return (
                StatusCodes.Status503ServiceUnavailable,
                "Database unavailable",
                "The database operation could not be completed.");
        }

        return null;
    }

    private static bool IsDatabaseException(Exception exception)
    {
        Exception? currentException = exception;

        while (currentException is not null)
        {
            if (currentException is DbUpdateException or NpgsqlException)
            {
                return true;
            }

            currentException = currentException.InnerException;
        }

        return false;
    }
}
