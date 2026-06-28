namespace ApplyWise.Api.Models.Requests;

public class AnalyzeJobMatchRequest
{
    public string ResumeText { get; init; } = string.Empty;

    public string JobDescription { get; init; } = string.Empty;
}
