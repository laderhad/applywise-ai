using ApplyWise.Api.Models.Requests;
using ApplyWise.Api.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ApplyWise.Api.Controllers;

[ApiController]
[Route("api/job-match")]
public class JobMatchController : ControllerBase
{
    [HttpPost("analyze")]
    public ActionResult<AnalyzeJobMatchResponse> Analyze(
        AnalyzeJobMatchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ResumeText))
        {
            return BadRequest(new { message = "Resume text is required." });
        }

        if (string.IsNullOrWhiteSpace(request.JobDescription))
        {
            return BadRequest(new { message = "Job description is required." });
        }

        var response = new AnalyzeJobMatchResponse
        {
            Summary = "The API is working. Ollama analysis will be added next."
        };

        return Ok(response);
    }
}
