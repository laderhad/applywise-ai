using ApplyWise.Api.Models.Responses;
using ApplyWise.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApplyWise.Api.Controllers;

[ApiController]
[Route("api/resumes")]
public sealed class ResumeController : ControllerBase
{
    private readonly ResumeUploadService _resumeUploadService;

    public ResumeController(ResumeUploadService resumeUploadService)
    {
        _resumeUploadService = resumeUploadService;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(PdfUploadValidator.MaxRequestSizeBytes)]
    public async Task<ActionResult<ResumeUploadResponse>> Upload(
        [FromForm] IFormFile? file,
        CancellationToken cancellationToken)
    {
        var response = await _resumeUploadService.UploadAsync(
            file,
            cancellationToken);

        return Ok(response);
    }
}
