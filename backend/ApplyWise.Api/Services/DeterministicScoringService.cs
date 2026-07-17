using ApplyWise.Api.Models.Responses;

namespace ApplyWise.Api.Services;

public sealed class DeterministicScoringService
{
    private static readonly string[] SkillLexicon =
    [
        ".net", "asp.net core", "c#", "entity framework", "ef core",
        "sql", "postgresql", "oracle", "sql server", "redis",
        "rabbitmq", "kafka", "docker", "kubernetes", "azure",
        "aws", "git", "github actions", "ci/cd", "rest api",
        "graphql", "microservices", "clean architecture", "unit testing",
        "integration testing", "xunit", "react", "typescript", "javascript",
        "html", "css", "vite", "redux", "signalr", "python",
        "machine learning", "llm", "ollama", "linux", "agile",
        "scrum", "oauth", "jwt"
    ];

    public ScoreBreakdownResponse Calculate(
        string resumeText,
        string jobDescription,
        int llmScore)
    {
        var normalizedResume = Normalize(resumeText);
        var normalizedJob = Normalize(jobDescription);

        var requiredSkills = SkillLexicon
            .Where(skill => ContainsSkill(normalizedJob, skill))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(skill => skill)
            .ToList();

        if (requiredSkills.Count == 0)
        {
            return new ScoreBreakdownResponse
            {
                DeterministicScore = llmScore,
                UsedLlmFallback = true,
                Explanation = "No supported skills were detected in the job description, so the model score was used as a fallback."
            };
        }

        var matchedSkills = requiredSkills
            .Where(skill => ContainsSkill(normalizedResume, skill))
            .ToList();

        var missingSkills = requiredSkills
            .Except(matchedSkills, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var score = (int)Math.Round(
            matchedSkills.Count * 100d / requiredSkills.Count,
            MidpointRounding.AwayFromZero);

        return new ScoreBreakdownResponse
        {
            DeterministicScore = score,
            MatchedSkillCount = matchedSkills.Count,
            RequiredSkillCount = requiredSkills.Count,
            MatchedSkills = matchedSkills,
            MissingSkills = missingSkills,
            UsedLlmFallback = false,
            Explanation = $"Matched {matchedSkills.Count} of {requiredSkills.Count} detected job skills."
        };
    }

    private static string Normalize(string value) =>
        value.Trim().ToLowerInvariant();

    private static bool ContainsSkill(string text, string skill) =>
        text.Contains(skill, StringComparison.OrdinalIgnoreCase);
}
