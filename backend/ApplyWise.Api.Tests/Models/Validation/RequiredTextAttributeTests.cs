using ApplyWise.Api.Models.Validation;
using Xunit;

namespace ApplyWise.Api.Tests.Models.Validation;

public sealed class RequiredTextAttributeTests
{
    private readonly RequiredTextAttribute _attribute = new();

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void RejectsMissingText(string? value)
    {
        Assert.False(_attribute.IsValid(value));
    }

    [Theory]
    [InlineData("ASP.NET Core")]
    [InlineData("  PostgreSQL  ")]
    public void AcceptsNonEmptyText(string value)
    {
        Assert.True(_attribute.IsValid(value));
    }
}
