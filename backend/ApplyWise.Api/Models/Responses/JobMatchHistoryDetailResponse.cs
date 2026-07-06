namespace ApplyWise.Api.Models.Responses;

public sealed class JobMatchHistoryDetailResponse
{
    public Guid Id { get; init; }

    public string ResumeText { get; init; } = string.Empty;

    public string JobDescription { get; init; } = string.Empty;

    public int MatchScore { get; init; }

    public List<string> StrongPoints { get; init; } = [];

    public List<string> WeakPoints { get; init; } = [];

    public List<string> MissingKeywords { get; init; } = [];

    public List<string> RecommendedBullets { get; init; } = [];

    public string CoverLetterDraft { get; init; } = string.Empty;

    public string LinkedinMessageDraft { get; init; } = string.Empty;

    public string Summary { get; init; } = string.Empty;

    public DateTimeOffset CreatedAt { get; init; }
}
