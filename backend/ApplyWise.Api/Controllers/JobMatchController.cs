using ApplyWise.Api.Models.Requests;
using ApplyWise.Api.Models.Responses;
using ApplyWise.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApplyWise.Api.Controllers;

[ApiController]
[Route("api/job-match")]
public class JobMatchController : ControllerBase
{
    private readonly OllamaService _ollamaService;

    public JobMatchController(OllamaService ollamaService)
    {
        _ollamaService = ollamaService;
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

            return Ok(response);
        }
        catch (OllamaServiceException exception)
        {
            return Problem(
                statusCode: StatusCodes.Status502BadGateway,
                title: "Ollama analysis failed",
                detail: exception.Message);
        }
    }
}
