using ApplyWise.Api.Models.Responses;
using AutoMapper;

namespace ApplyWise.Api.Services;

public sealed class ResumeUploadService
{
    private readonly PdfUploadValidator _pdfUploadValidator;
    private readonly PdfTextExtractor _pdfTextExtractor;
    private readonly IMapper _mapper;

    public ResumeUploadService(
        PdfUploadValidator pdfUploadValidator,
        PdfTextExtractor pdfTextExtractor,
        IMapper mapper)
    {
        _pdfUploadValidator = pdfUploadValidator;
        _pdfTextExtractor = pdfTextExtractor;
        _mapper = mapper;
    }

    public async Task<ResumeUploadResponse> UploadAsync(
        IFormFile? file,
        CancellationToken cancellationToken)
    {
        var validationError =
            await _pdfUploadValidator.GetValidationErrorAsync(
                file,
                cancellationToken);

        if (validationError is not null)
        {
            throw new ResumeUploadValidationException(validationError);
        }

        using var stream = file!.OpenReadStream();
        var extraction = _pdfTextExtractor.Extract(stream);

        var result = new ResumeUploadResult(
            Path.GetFileName(file.FileName),
            file.Length,
            extraction.PageCount,
            extraction.Text);

        return _mapper.Map<ResumeUploadResponse>(result);
    }
}
