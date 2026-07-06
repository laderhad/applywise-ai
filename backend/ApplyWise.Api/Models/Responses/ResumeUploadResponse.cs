namespace ApplyWise.Api.Models.Responses;

public sealed class ResumeUploadResponse
{
    public string FileName { get; init; } = string.Empty;

    public long SizeBytes { get; init; }
}
