namespace ApplyWise.Api.Services;

public sealed record ResumeUploadResult(
    string FileName,
    long SizeBytes,
    int PageCount,
    string ExtractedText);
