using ApplyWise.Api.Data;
using ApplyWise.Api.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace ApplyWise.Api.Services;

public sealed class JobMatchHistoryService
{
    private const int HistoryLimit = 50;

    private readonly ApplyWiseDbContext _dbContext;

    public JobMatchHistoryService(ApplyWiseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<
        JobMatchHistoryItemResponse>> GetHistoryAsync(
        CancellationToken cancellationToken)
    {
        return await _dbContext.JobMatchAnalyses
            .AsNoTracking()
            .OrderByDescending(item => item.CreatedAt)
            .Select(item => new JobMatchHistoryItemResponse
            {
                Id = item.Id,
                MatchScore = item.MatchScore,
                Summary = item.Summary,
                CreatedAt = item.CreatedAt
            })
            .Take(HistoryLimit)
            .ToListAsync(cancellationToken);
    }

    public async Task<
        JobMatchHistoryDetailResponse?> GetHistoryDetailAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _dbContext.JobMatchAnalyses
            .AsNoTracking()
            .Where(item => item.Id == id)
            .Select(item => new JobMatchHistoryDetailResponse
            {
                Id = item.Id,
                ResumeText = item.ResumeText,
                JobDescription = item.JobDescription,
                MatchScore = item.MatchScore,
                StrongPoints = item.StrongPoints,
                WeakPoints = item.WeakPoints,
                MissingKeywords = item.MissingKeywords,
                RecommendedBullets = item.RecommendedBullets,
                CoverLetterDraft = item.CoverLetterDraft,
                LinkedinMessageDraft = item.LinkedinMessageDraft,
                Summary = item.Summary,
                CreatedAt = item.CreatedAt
            })
            .SingleOrDefaultAsync(cancellationToken);
    }
}
