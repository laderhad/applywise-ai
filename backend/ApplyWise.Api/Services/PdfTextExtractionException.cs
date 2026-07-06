namespace ApplyWise.Api.Services;

public sealed class PdfTextExtractionException : Exception
{
    public PdfTextExtractionException(
        string message,
        Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
