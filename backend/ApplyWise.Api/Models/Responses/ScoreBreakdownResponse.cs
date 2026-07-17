namespace ApplyWise.Api.Models.Responses;

public sealed class ScoreBreakdownResponse
{
    public int DeterministicScore { get; init; }

    public int MatchedSkillCount { get; init; }

    public int RequiredSkillCount { get; init; }

    public List<string> MatchedSkills { get; init; } = [];

    public List<string> MissingSkills { get; init; } = [];

    public bool UsedLlmFallback { get; init; }

    public string Explanation { get; init; } = string.Empty;
}
