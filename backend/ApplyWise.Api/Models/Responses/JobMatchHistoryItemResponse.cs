namespace ApplyWise.Api.Models.Responses;

public sealed class JobMatchHistoryItemResponse
{
    public Guid Id { get; init; }

    public int MatchScore { get; init; }

    public string Summary { get; init; } = string.Empty;

    public DateTimeOffset CreatedAt { get; init; }
}
