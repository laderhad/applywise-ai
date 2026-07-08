using ApplyWise.Api.Prompts;
using Xunit;

namespace ApplyWise.Api.Tests.Prompts;

public sealed class JobMatchPromptBuilderTests
{
    [Fact]
    public void UsesEnglishResponseLanguageByDefault()
    {
        var prompt = JobMatchPromptBuilder.Build(
            "Built REST APIs.",
            "Requires REST API experience.");

        Assert.Contains(
            "Write all descriptive string values in English.",
            prompt);
    }

    [Fact]
    public void UsesTurkishResponseLanguageWhenRequested()
    {
        var prompt = JobMatchPromptBuilder.Build(
            "REST API geliştirdi.",
            "REST API deneyimi gerekiyor.",
            "tr");

        Assert.Contains(
            "Write all descriptive string values in Turkish.",
            prompt);
        Assert.Contains(
            "Keep JSON property names exactly as shown in the schema.",
            prompt);
    }
}
