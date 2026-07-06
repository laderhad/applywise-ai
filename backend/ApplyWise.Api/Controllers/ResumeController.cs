using ApplyWise.Api.Models.Responses;
using ApplyWise.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApplyWise.Api.Controllers;

[ApiController]
[Route("api/resumes")]
public sealed class ResumeController : ControllerBase
{
    private readonly PdfUploadValidator _pdfUploadValidator;
    private readonly PdfTextExtractor _pdfTextExtractor;

    public ResumeController(
        PdfUploadValidator pdfUploadValidator,
        PdfTextExtractor pdfTextExtractor)
    {
        _pdfUploadValidator = pdfUploadValidator;
        _pdfTextExtractor = pdfTextExtractor;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(PdfUploadValidator.MaxRequestSizeBytes)]
    public async Task<ActionResult<ResumeUploadResponse>> Upload(
        [FromForm] IFormFile? file,
        CancellationToken cancellationToken)
    {
        var validationError =
            await _pdfUploadValidator.GetValidationErrorAsync(
                file,
                cancellationToken);

        if (validationError is not null)
        {
            return BadRequest(new { message = validationError });
        }

        using var stream = file!.OpenReadStream();
        var extraction = _pdfTextExtractor.Extract(stream);

        return Ok(new ResumeUploadResponse
        {
            FileName = Path.GetFileName(file.FileName),
            SizeBytes = file.Length,
            PageCount = extraction.PageCount,
            ExtractedText = extraction.Text
        });
    }
}
