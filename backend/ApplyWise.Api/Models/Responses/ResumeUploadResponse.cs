namespace ApplyWise.Api.Models.Responses;

public sealed class ResumeUploadResponse
{
    public string FileName { get; init; } = string.Empty;

    public long SizeBytes { get; init; }

    public int PageCount { get; init; }

    public string ExtractedText { get; init; } = string.Empty;
}
