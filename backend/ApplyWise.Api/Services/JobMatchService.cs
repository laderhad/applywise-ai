using ApplyWise.Api.Data;
using ApplyWise.Api.Data.Entities;
using ApplyWise.Api.Models.Responses;
using AutoMapper;

namespace ApplyWise.Api.Services;

public sealed class JobMatchService
{
    private readonly OllamaService _ollamaService;
    private readonly ApplyWiseDbContext _dbContext;
    private readonly IMapper _mapper;

    public JobMatchService(
        OllamaService ollamaService,
        ApplyWiseDbContext dbContext,
        IMapper mapper)
    {
        _ollamaService = ollamaService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<AnalyzeJobMatchResponse> AnalyzeAsync(
        string resumeText,
        string jobDescription,
        string language,
        CancellationToken cancellationToken)
    {
        var response = await _ollamaService.AnalyzeAsync(
            resumeText,
            jobDescription,
            language,
            cancellationToken);

        var analysis = _mapper.Map<JobMatchAnalysis>(response);
        analysis.ResumeText = resumeText;
        analysis.JobDescription = jobDescription;

        _dbContext.JobMatchAnalyses.Add(analysis);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return response;
    }
}
