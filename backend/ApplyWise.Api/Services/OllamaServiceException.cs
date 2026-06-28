namespace ApplyWise.Api.Services;

public sealed class OllamaServiceException : Exception
{
    public OllamaServiceException(string message)
        : base(message)
    {
    }

    public OllamaServiceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
