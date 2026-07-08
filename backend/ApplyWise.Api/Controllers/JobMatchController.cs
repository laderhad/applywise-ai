using ApplyWise.Api.Models.Requests;
using ApplyWise.Api.Models.Responses;
using ApplyWise.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApplyWise.Api.Controllers;

[ApiController]
[Route("api/job-match")]
public class JobMatchController : ControllerBase
{
    private readonly JobMatchService _jobMatchService;
    private readonly JobMatchHistoryService _historyService;

    public JobMatchController(
        JobMatchService jobMatchService,
        JobMatchHistoryService historyService)
    {
        _jobMatchService = jobMatchService;
        _historyService = historyService;
    }

    [HttpGet("history")]
    public async Task<ActionResult<IReadOnlyList<
        JobMatchHistoryItemResponse>>> GetHistory(
        CancellationToken cancellationToken)
    {
        var history = await _historyService.GetHistoryAsync(
            cancellationToken);

        return Ok(history);
    }

    [HttpGet("history/{id:guid}")]
    public async Task<ActionResult<
        JobMatchHistoryDetailResponse>> GetHistoryDetail(
        Guid id,
        CancellationToken cancellationToken)
    {
        var analysis = await _historyService.GetHistoryDetailAsync(
            id,
            cancellationToken);

        if (analysis is null)
        {
            return NotFound(new
            {
                message = "Analysis not found."
            });
        }

        return Ok(analysis);
    }

    [HttpPost("analyze")]
    public async Task<ActionResult<AnalyzeJobMatchResponse>> Analyze(
        AnalyzeJobMatchRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _jobMatchService.AnalyzeAsync(
            request.ResumeText,
            request.JobDescription,
            request.Language,
            cancellationToken);

        return Ok(response);
    }
}
