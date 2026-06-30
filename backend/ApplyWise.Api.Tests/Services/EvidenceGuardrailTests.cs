using ApplyWise.Api.Models.Responses;
using ApplyWise.Api.Services;
using Xunit;

namespace ApplyWise.Api.Tests.Services;

public class EvidenceGuardrailTests
{
    [Fact]
    public void KeepsBulletWhenMaterialClaimsExistInResume()
    {
        const string resume =
            "Built REST APIs using C# and ASP.NET Core.";
        var response = CreateResponse(
            "Developed REST APIs using C# and ASP.NET Core.");

        var result = EvidenceGuardrail.FilterUnsupportedClaims(
            resume,
            response);

        Assert.Single(result.RecommendedBullets);
    }

    [Fact]
    public void RemovesBulletWhenTechnologyIsAbsentFromResume()
    {
        const string resume =
            "Built REST APIs using C# and ASP.NET Core.";
        var response = CreateResponse(
            "Utilized Docker for containerization of API services.");

        var result = EvidenceGuardrail.FilterUnsupportedClaims(
            resume,
            response);

        Assert.Empty(result.RecommendedBullets);
    }

    [Fact]
    public void RemovesBulletsWithUnsupportedResponsibilities()
    {
        const string resume =
            "Worked with PostgreSQL and improved API response times.";
        var response = CreateResponse(
            "Collaborated with product teams to improve API performance.",
            "Designed and implemented a PostgreSQL database schema.");

        var result = EvidenceGuardrail.FilterUnsupportedClaims(
            resume,
            response);

        Assert.Empty(result.RecommendedBullets);
    }

    private static AnalyzeJobMatchResponse CreateResponse(
        params string[] recommendedBullets)
    {
        return new AnalyzeJobMatchResponse
        {
            RecommendedBullets = [.. recommendedBullets]
        };
    }
}
