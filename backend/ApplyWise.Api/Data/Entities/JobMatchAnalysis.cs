namespace ApplyWise.Api.Data.Entities;

public sealed class JobMatchAnalysis
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string ResumeText { get; set; } = string.Empty;

    public string JobDescription { get; set; } = string.Empty;

    public int MatchScore { get; set; }

    public List<string> StrongPoints { get; set; } = [];

    public List<string> WeakPoints { get; set; } = [];

    public List<string> MissingKeywords { get; set; } = [];

    public List<string> RecommendedBullets { get; set; } = [];

    public string CoverLetterDraft { get; set; } = string.Empty;

    public string LinkedinMessageDraft { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
