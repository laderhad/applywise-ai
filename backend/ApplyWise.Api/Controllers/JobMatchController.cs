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

    [HttpGet("history")]
    public async Task<ActionResult<IReadOnlyList<
        JobMatchHistoryItemResponse>>> GetHistory(
        CancellationToken cancellationToken)
    {
        try
        {
            var history = await _dbContext.JobMatchAnalyses
                .AsNoTracking()
                .OrderByDescending(item => item.CreatedAt)
                .Select(item => new JobMatchHistoryItemResponse
                {
                    Id = item.Id,
                    MatchScore = item.MatchScore,
                    Summary = item.Summary,
                    CreatedAt = item.CreatedAt
                })
                .Take(50)
                .ToListAsync(cancellationToken);

            return Ok(history);
        }
        catch (Exception exception)
            when (IsDatabaseException(exception))
        {
            _logger.LogError(
                exception,
                "The job match history could not be loaded.");

            return Problem(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                title: "Analysis history unavailable",
                detail: "The analysis history could not be loaded.");
        }
    }

    [HttpGet("history/{id:guid}")]
    public async Task<ActionResult<
        JobMatchHistoryDetailResponse>> GetHistoryDetail(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var analysis = await _dbContext.JobMatchAnalyses
                .AsNoTracking()
                .Where(item => item.Id == id)
                .Select(item => new JobMatchHistoryDetailResponse
                {
                    Id = item.Id,
                    ResumeText = item.ResumeText,
                    JobDescription = item.JobDescription,
                    MatchScore = item.MatchScore,
                    StrongPoints = item.StrongPoints,
                    WeakPoints = item.WeakPoints,
                    MissingKeywords = item.MissingKeywords,
                    RecommendedBullets = item.RecommendedBullets,
                    CoverLetterDraft = item.CoverLetterDraft,
                    LinkedinMessageDraft = item.LinkedinMessageDraft,
                    Summary = item.Summary,
                    CreatedAt = item.CreatedAt
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (analysis is null)
            {
                return NotFound(new
                {
                    message = "Analysis not found."
                });
            }

            return Ok(analysis);
        }
        catch (Exception exception)
            when (IsDatabaseException(exception))
        {
            _logger.LogError(
                exception,
                "The job match analysis detail could not be loaded.");

            return Problem(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                title: "Analysis history unavailable",
                detail: "The analysis detail could not be loaded.");
        }
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
