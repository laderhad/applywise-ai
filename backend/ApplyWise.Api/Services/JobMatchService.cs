using ApplyWise.Api.Data;
using ApplyWise.Api.Data.Entities;
using ApplyWise.Api.Models.Responses;

namespace ApplyWise.Api.Services;

public sealed class JobMatchService
{
    private readonly OllamaService _ollamaService;
    private readonly ApplyWiseDbContext _dbContext;

    public JobMatchService(
        OllamaService ollamaService,
        ApplyWiseDbContext dbContext)
    {
        _ollamaService = ollamaService;
        _dbContext = dbContext;
    }

    public async Task<AnalyzeJobMatchResponse> AnalyzeAsync(
        string resumeText,
        string jobDescription,
        CancellationToken cancellationToken)
    {
        var response = await _ollamaService.AnalyzeAsync(
            resumeText,
            jobDescription,
            cancellationToken);

        _dbContext.JobMatchAnalyses.Add(
            CreateAnalysis(resumeText, jobDescription, response));

        await _dbContext.SaveChangesAsync(cancellationToken);

        return response;
    }

    private static JobMatchAnalysis CreateAnalysis(
        string resumeText,
        string jobDescription,
        AnalyzeJobMatchResponse response)
    {
        return new JobMatchAnalysis
        {
            ResumeText = resumeText,
            JobDescription = jobDescription,
            MatchScore = response.MatchScore,
            StrongPoints = [.. response.StrongPoints],
            WeakPoints = [.. response.WeakPoints],
            MissingKeywords = [.. response.MissingKeywords],
            RecommendedBullets = [.. response.RecommendedBullets],
            CoverLetterDraft = response.CoverLetterDraft,
            LinkedinMessageDraft = response.LinkedinMessageDraft,
            Summary = response.Summary
        };
    }
}
