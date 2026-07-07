using ApplyWise.Api.Data;
using ApplyWise.Api.Models.Responses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ApplyWise.Api.Services;

public sealed class JobMatchHistoryService
{
    private const int HistoryLimit = 50;

    private readonly ApplyWiseDbContext _dbContext;
    private readonly IMapper _mapper;

    public JobMatchHistoryService(
        ApplyWiseDbContext dbContext,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<
        JobMatchHistoryItemResponse>> GetHistoryAsync(
        CancellationToken cancellationToken)
    {
        return await _dbContext.JobMatchAnalyses
            .AsNoTracking()
            .OrderByDescending(item => item.CreatedAt)
            .Take(HistoryLimit)
            .ProjectTo<JobMatchHistoryItemResponse>(
                _mapper.ConfigurationProvider)
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
            .ProjectTo<JobMatchHistoryDetailResponse>(
                _mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(cancellationToken);
    }
}
