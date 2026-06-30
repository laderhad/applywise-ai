using System.Text.RegularExpressions;
using ApplyWise.Api.Models.Responses;

namespace ApplyWise.Api.Services;

public static class EvidenceGuardrail
{
    private static readonly Regex TokenPattern =
        new(@"[\p{L}\p{N}+#.]+", RegexOptions.Compiled);

    private static readonly HashSet<string> IgnoredTokens =
    [
        "a",
        "an",
        "and",
        "are",
        "as",
        "at",
        "be",
        "by",
        "for",
        "from",
        "had",
        "has",
        "have",
        "in",
        "into",
        "is",
        "of",
        "on",
        "or",
        "the",
        "their",
        "through",
        "to",
        "was",
        "were",
        "with"
    ];

    private static readonly HashSet<string> FramingTokens =
    [
        "ability",
        "background",
        "candidate",
        "demonstrat",
        "experience",
        "expertise",
        "familiarity",
        "knowledge",
        "proficiency",
        "proficient",
        "relevant",
        "skill",
        "strong",
        "successfully"
    ];

    private static readonly string[][] ActionVerbGroups =
    [
        ["build", "creat", "design", "develop", "implement"],
        ["leverag", "use", "utiliz", "work"],
        ["enhanc", "improv", "increas", "optimiz", "reduc"],
        ["creat", "develop", "write"]
    ];

    public static AnalyzeJobMatchResponse FilterUnsupportedClaims(
        string resumeText,
        AnalyzeJobMatchResponse response)
    {
        var evidenceTokens = BuildEvidenceTokens(resumeText);

        return new AnalyzeJobMatchResponse
        {
            MatchScore = response.MatchScore,
            StrongPoints = FilterItems(
                response.StrongPoints,
                evidenceTokens),
            WeakPoints = [.. response.WeakPoints],
            MissingKeywords = [.. response.MissingKeywords],
            RecommendedBullets = FilterItems(
                response.RecommendedBullets,
                evidenceTokens),
            CoverLetterDraft = response.CoverLetterDraft,
            LinkedinMessageDraft = response.LinkedinMessageDraft,
            Summary = response.Summary
        };
    }

    private static HashSet<string> BuildEvidenceTokens(string resumeText)
    {
        var evidenceTokens = Tokenize(resumeText).ToHashSet();
        var originalTokens = evidenceTokens.ToHashSet();

        foreach (var verbGroup in ActionVerbGroups)
        {
            if (verbGroup.Any(originalTokens.Contains))
            {
                evidenceTokens.UnionWith(verbGroup);
            }
        }

        return evidenceTokens;
    }

    private static List<string> FilterItems(
        IEnumerable<string> items,
        HashSet<string> evidenceTokens)
    {
        return items
            .Where(item => IsSupported(item, evidenceTokens))
            .ToList();
    }

    private static bool IsSupported(
        string item,
        HashSet<string> evidenceTokens)
    {
        var hasEvidence = false;

        foreach (var token in Tokenize(item))
        {
            if (IgnoredTokens.Contains(token) ||
                FramingTokens.Contains(token))
            {
                continue;
            }

            if (!evidenceTokens.Contains(token))
            {
                return false;
            }

            hasEvidence = true;
        }

        return hasEvidence;
    }

    private static IEnumerable<string> Tokenize(string text)
    {
        return TokenPattern
            .Matches(text.ToLowerInvariant())
            .Select(match => NormalizeToken(match.Value))
            .Where(token => token.Length > 2 || token.Any(char.IsDigit));
    }

    private static string NormalizeToken(string token)
    {
        token = token.Trim('.');

        if (token is "built")
        {
            return "build";
        }

        if (token is "using" or "used")
        {
            return "use";
        }

        if (token is "wrote" or "written")
        {
            return "write";
        }

        if (token.EndsWith("ing", StringComparison.Ordinal) &&
            token.Length > 5)
        {
            return token[..^3];
        }

        if (token.EndsWith("ed", StringComparison.Ordinal) &&
            token.Length > 4)
        {
            return token[..^2];
        }

        if (token.EndsWith('s') && token.Length > 3)
        {
            return token[..^1];
        }

        return token;
    }
}
