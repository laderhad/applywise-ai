namespace ApplyWise.Api.Models.Responses;

public class AnalyzeJobMatchResponse
{
    public int MatchScore { get; init; }

    public List<string> StrongPoints { get; init; } = [];

    public List<string> WeakPoints { get; init; } = [];

    public List<string> MissingKeywords { get; init; } = [];

    public List<string> RecommendedBullets { get; init; } = [];

    public string CoverLetterDraft { get; init; } = string.Empty;

    public string LinkedinMessageDraft { get; init; } = string.Empty;

    public string Summary { get; init; } = string.Empty;
}
