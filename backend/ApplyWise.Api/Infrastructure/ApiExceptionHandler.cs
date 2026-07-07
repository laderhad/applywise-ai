using System.Text.Json;
using ApplyWise.Api.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using UglyToad.PdfPig.Core;

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
        var problem = GetProblem(exception, httpContext);

        if (problem is null)
        {
            return false;
        }

        var logLevel = problem.Value.StatusCode >= 500
            ? LogLevel.Error
            : LogLevel.Warning;

        _logger.Log(
            logLevel,
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
        string Detail)? GetProblem(
        Exception exception,
        HttpContext httpContext)
    {
        if (exception is OperationCanceledException &&
            httpContext.RequestAborted.IsCancellationRequested)
        {
            return null;
        }

        if (exception is ResumeUploadValidationException)
        {
            return (
                StatusCodes.Status400BadRequest,
                "Resume upload validation failed",
                exception.Message);
        }

        if (exception is PdfTextExtractionException)
        {
            return (
                StatusCodes.Status422UnprocessableEntity,
                "PDF text extraction failed",
                exception.Message);
        }

        if (exception is PdfDocumentFormatException)
        {
            return (
                StatusCodes.Status422UnprocessableEntity,
                "PDF text extraction failed",
                "The PDF could not be read.");
        }

        if (exception is OllamaServiceException)
        {
            return (
                StatusCodes.Status502BadGateway,
                "Ollama analysis failed",
                exception.Message);
        }

        if (exception is TaskCanceledException)
        {
            return (
                StatusCodes.Status504GatewayTimeout,
                "Ollama analysis timed out",
                "Ollama did not respond before the request timed out.");
        }

        if (exception is HttpRequestException)
        {
            return (
                StatusCodes.Status502BadGateway,
                "Ollama analysis failed",
                "Could not connect to Ollama. "
                + "Make sure it is running locally.");
        }

        if (exception is JsonException)
        {
            return (
                StatusCodes.Status502BadGateway,
                "Ollama analysis failed",
                "Ollama returned JSON in an unexpected format.");
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
