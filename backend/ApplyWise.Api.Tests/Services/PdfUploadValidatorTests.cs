using ApplyWise.Api.Services;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace ApplyWise.Api.Tests.Services;

public class PdfUploadValidatorTests
{
    private readonly PdfUploadValidator _validator = new();

    [Fact]
    public async Task AcceptsPdfExtensionAndSignature()
    {
        var file = CreateFile(
            "resume.pdf",
            "%PDF-1.7 test content"u8.ToArray());

        var error = await _validator.GetValidationErrorAsync(
            file,
            CancellationToken.None);

        Assert.Null(error);
    }

    [Fact]
    public async Task RejectsEmptyFile()
    {
        var file = CreateFile("resume.pdf", []);

        var error = await _validator.GetValidationErrorAsync(
            file,
            CancellationToken.None);

        Assert.Equal("A PDF resume is required.", error);
    }

    [Fact]
    public async Task RejectsNonPdfExtension()
    {
        var file = CreateFile(
            "resume.txt",
            "%PDF-1.7 test content"u8.ToArray());

        var error = await _validator.GetValidationErrorAsync(
            file,
            CancellationToken.None);

        Assert.Equal("Only PDF files are supported.", error);
    }

    [Fact]
    public async Task RejectsFileWithoutPdfSignature()
    {
        var file = CreateFile(
            "resume.pdf",
            "not a real PDF"u8.ToArray());

        var error = await _validator.GetValidationErrorAsync(
            file,
            CancellationToken.None);

        Assert.Equal("The uploaded file is not a valid PDF.", error);
    }

    [Fact]
    public async Task RejectsFileLargerThanFiveMegabytes()
    {
        var content = new byte[PdfUploadValidator.MaxFileSizeBytes + 1];
        "%PDF-"u8.CopyTo(content);
        var file = CreateFile("resume.pdf", content);

        var error = await _validator.GetValidationErrorAsync(
            file,
            CancellationToken.None);

        Assert.Equal("The PDF resume must be 5 MB or smaller.", error);
    }

    private static FormFile CreateFile(
        string fileName,
        byte[] content)
    {
        var stream = new MemoryStream(content);

        return new FormFile(
            stream,
            0,
            stream.Length,
            "file",
            fileName);
    }
}
