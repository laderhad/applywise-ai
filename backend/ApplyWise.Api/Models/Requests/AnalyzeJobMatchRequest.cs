using ApplyWise.Api.Models.Validation;

namespace ApplyWise.Api.Models.Requests;

public class AnalyzeJobMatchRequest
{
    [RequiredText(ErrorMessage = "Resume text is required.")]
    public string ResumeText { get; init; } = string.Empty;

    [RequiredText(ErrorMessage = "Job description is required.")]
    public string JobDescription { get; init; } = string.Empty;

    public string Language { get; init; } = "en";
}
