using ApplyWise.Api.Data;
using ApplyWise.Api.Data.Entities;
using ApplyWise.Api.Models.Responses;
using AutoMapper;

namespace ApplyWise.Api.Services;

public sealed class JobMatchService
{
    private readonly OllamaService _ollamaService;
    private readonly DeterministicScoringService _scoringService;
    private readonly ApplyWiseDbContext _dbContext;
    private readonly IMapper _mapper;

    public JobMatchService(
        OllamaService ollamaService,
        DeterministicScoringService scoringService,
        ApplyWiseDbContext dbContext,
        IMapper mapper)
    {
        _ollamaService = ollamaService;
        _scoringService = scoringService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<AnalyzeJobMatchResponse> AnalyzeAsync(
        string resumeText,
        string jobDescription,
        string language,
        CancellationToken cancellationToken)
    {
        var llmResponse = await _ollamaService.AnalyzeAsync(
            resumeText,
            jobDescription,
            language,
            cancellationToken);

        var breakdown = _scoringService.Calculate(
            resumeText,
            jobDescription,
            llmResponse.MatchScore);

        var response = new AnalyzeJobMatchResponse
        {
            MatchScore = breakdown.DeterministicScore,
            ScoreBreakdown = breakdown,
            StrongPoints = llmResponse.StrongPoints,
            WeakPoints = llmResponse.WeakPoints,
            MissingKeywords = llmResponse.MissingKeywords,
            RecommendedBullets = llmResponse.RecommendedBullets,
            CoverLetterDraft = llmResponse.CoverLetterDraft,
            LinkedinMessageDraft = llmResponse.LinkedinMessageDraft,
            Summary = llmResponse.Summary
        };

        var analysis = _mapper.Map<JobMatchAnalysis>(response);
        analysis.ResumeText = resumeText;
        analysis.JobDescription = jobDescription;

        _dbContext.JobMatchAnalyses.Add(analysis);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return response;
    }
}
