using ApplyWise.Api.Services;
using Xunit;

namespace ApplyWise.Api.Tests.Services;

public sealed class DeterministicScoringServiceTests
{
    private readonly DeterministicScoringService _service = new();

    [Fact]
    public void Calculate_ReturnsRatioOfMatchedRequiredSkills()
    {
        var result = _service.Calculate(
            "C# developer experienced with ASP.NET Core, PostgreSQL and Docker.",
            "We need C#, ASP.NET Core, PostgreSQL, Docker and Redis.",
            llmScore: 12);

        Assert.Equal(80, result.DeterministicScore);
        Assert.Equal(4, result.MatchedSkillCount);
        Assert.Equal(5, result.RequiredSkillCount);
        Assert.Contains("redis", result.MissingSkills);
        Assert.False(result.UsedLlmFallback);
    }

    [Fact]
    public void Calculate_UsesLlmFallback_WhenNoSupportedSkillIsDetected()
    {
        var result = _service.Calculate(
            "Experienced engineer.",
            "We need a thoughtful teammate with strong communication.",
            llmScore: 73);

        Assert.Equal(73, result.DeterministicScore);
        Assert.True(result.UsedLlmFallback);
        Assert.Empty(result.MatchedSkills);
        Assert.Empty(result.MissingSkills);
    }
}
