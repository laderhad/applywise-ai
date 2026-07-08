using System.ComponentModel.DataAnnotations;

namespace ApplyWise.Api.Models.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class RequiredTextAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        return value is string text &&
            !string.IsNullOrWhiteSpace(text);
    }
}
