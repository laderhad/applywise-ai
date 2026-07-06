using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace ApplyWise.Api.Services;

public sealed class PdfTextExtractor
{
    public PdfTextExtraction Extract(Stream stream)
    {
        try
        {
            using var document = PdfDocument.Open(stream);
            var textBuilder = new StringBuilder();

            foreach (var page in document.GetPages())
            {
                var pageText =
                    ContentOrderTextExtractor.GetText(page).Trim();

                if (pageText.Length > 0)
                {
                    textBuilder.AppendLine(pageText);
                }
            }

            var text = textBuilder.ToString().Trim();

            if (text.Length == 0)
            {
                throw new PdfTextExtractionException(
                    "No selectable text was found in the PDF. "
                    + "Scanned PDFs are not supported yet.");
            }

            return new PdfTextExtraction(
                text,
                document.NumberOfPages);
        }
        catch (PdfDocumentFormatException exception)
        {
            throw new PdfTextExtractionException(
                "The PDF could not be read.",
                exception);
        }
    }
}

public sealed record PdfTextExtraction(
    string Text,
    int PageCount);
