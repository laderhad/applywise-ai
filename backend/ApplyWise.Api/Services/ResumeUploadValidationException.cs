namespace ApplyWise.Api.Services;

public sealed class ResumeUploadValidationException : Exception
{
    public ResumeUploadValidationException(string message)
        : base(message)
    {
    }
}
