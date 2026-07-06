using Microsoft.AspNetCore.Http;

namespace ApplyWise.Api.Services;

public sealed class PdfUploadValidator
{
    public const long MaxFileSizeBytes = 5 * 1024 * 1024;

    public const long MaxRequestSizeBytes =
        MaxFileSizeBytes + (64 * 1024);

    private static readonly byte[] PdfSignature = "%PDF-"u8.ToArray();

    public async Task<string?> GetValidationErrorAsync(
        IFormFile? file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return "A PDF resume is required.";
        }

        if (file.Length > MaxFileSizeBytes)
        {
            return "The PDF resume must be 5 MB or smaller.";
        }

        if (!string.Equals(
                Path.GetExtension(file.FileName),
                ".pdf",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Only PDF files are supported.";
        }

        await using var stream = file.OpenReadStream();
        var header = new byte[PdfSignature.Length];
        var bytesRead = await stream.ReadAsync(
            header,
            cancellationToken);

        if (bytesRead != PdfSignature.Length
            || !header.AsSpan().SequenceEqual(PdfSignature))
        {
            return "The uploaded file is not a valid PDF.";
        }

        return null;
    }
}
