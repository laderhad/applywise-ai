using ApplyWise.Api.Data;
using ApplyWise.Api.Data.Entities;
using ApplyWise.Api.Models.Requests;
using ApplyWise.Api.Models.Responses;
using ApplyWise.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace ApplyWise.Api.Controllers;

[ApiController]
[Route("api/job-match")]
public class JobMatchController : ControllerBase
{
    private readonly OllamaService _ollamaService;
    private readonly ApplyWiseDbContext _dbContext;
    private readonly ILogger<JobMatchController> _logger;

    public JobMatchController(
        OllamaService ollamaService,
        ApplyWiseDbContext dbContext,
        ILogger<JobMatchController> logger)
    {
        _ollamaService = ollamaService;
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpPost("analyze")]
    public async Task<ActionResult<AnalyzeJobMatchResponse>> Analyze(
        AnalyzeJobMatchRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ResumeText))
        {
            return BadRequest(new { message = "Resume text is required." });
        }

        if (string.IsNullOrWhiteSpace(request.JobDescription))
        {
            return BadRequest(new { message = "Job description is required." });
        }

        try
        {
            var response = await _ollamaService.AnalyzeAsync(
                request.ResumeText,
                request.JobDescription,
                cancellationToken);

            var analysis = new JobMatchAnalysis
            {
                ResumeText = request.ResumeText,
                JobDescription = request.JobDescription,
                MatchScore = response.MatchScore,
                StrongPoints = [.. response.StrongPoints],
                WeakPoints = [.. response.WeakPoints],
                MissingKeywords = [.. response.MissingKeywords],
                RecommendedBullets = [.. response.RecommendedBullets],
                CoverLetterDraft = response.CoverLetterDraft,
                LinkedinMessageDraft = response.LinkedinMessageDraft,
                Summary = response.Summary
            };

            _dbContext.JobMatchAnalyses.Add(analysis);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Ok(response);
        }
        catch (OllamaServiceException exception)
        {
            return Problem(
                statusCode: StatusCodes.Status502BadGateway,
                title: "Ollama analysis failed",
                detail: exception.Message);
        }
        catch (Exception exception)
            when (IsDatabaseException(exception))
        {
            _logger.LogError(
                exception,
                "The job match analysis could not be saved.");

            return Problem(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                title: "Analysis storage failed",
                detail: "The analysis completed but could not be saved.");
        }
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
