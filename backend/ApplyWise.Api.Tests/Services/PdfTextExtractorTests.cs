using ApplyWise.Api.Services;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Fonts.Standard14Fonts;
using UglyToad.PdfPig.Writer;
using Xunit;

namespace ApplyWise.Api.Tests.Services;

public class PdfTextExtractorTests
{
    private readonly PdfTextExtractor _extractor = new();

    [Fact]
    public void ExtractsTextAndPageCount()
    {
        using var stream = CreatePdf(
            "ASP.NET Core and PostgreSQL experience");

        var result = _extractor.Extract(stream);

        Assert.Equal(1, result.PageCount);
        Assert.Contains(
            "ASP.NET Core and PostgreSQL experience",
            result.Text);
    }

    [Fact]
    public void RejectsPdfWithoutSelectableText()
    {
        using var stream = CreatePdf();

        var exception = Assert.Throws<PdfTextExtractionException>(
            () => _extractor.Extract(stream));

        Assert.Equal(
            "No selectable text was found in the PDF. "
            + "Scanned PDFs are not supported yet.",
            exception.Message);
    }

    [Fact]
    public void RejectsMalformedPdf()
    {
        using var stream = new MemoryStream(
            "%PDF-not-a-document"u8.ToArray());

        var exception = Assert.Throws<PdfTextExtractionException>(
            () => _extractor.Extract(stream));

        Assert.Equal("The PDF could not be read.", exception.Message);
    }

    private static MemoryStream CreatePdf(string? text = null)
    {
        var builder = new PdfDocumentBuilder();
        var page = builder.AddPage(PageSize.A4);

        if (text is not null)
        {
            var font = builder.AddStandard14Font(
                Standard14Font.Helvetica);

            page.AddText(
                text,
                12,
                new PdfPoint(50, 750),
                font);
        }

        return new MemoryStream(builder.Build());
    }
}
