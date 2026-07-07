using ApplyWise.Api.Data.Entities;
using ApplyWise.Api.Models.Responses;
using ApplyWise.Api.Services;
using AutoMapper;

namespace ApplyWise.Api.Mappings;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<JobMatchAnalysis, JobMatchHistoryItemResponse>();
        CreateMap<JobMatchAnalysis, JobMatchHistoryDetailResponse>();
        CreateMap<ResumeUploadResult, ResumeUploadResponse>();

        CreateMap<AnalyzeJobMatchResponse, JobMatchAnalysis>()
            .ForMember(destination => destination.Id, options => options.Ignore())
            .ForMember(
                destination => destination.ResumeText,
                options => options.Ignore())
            .ForMember(
                destination => destination.JobDescription,
                options => options.Ignore())
            .ForMember(
                destination => destination.CreatedAt,
                options => options.Ignore());
    }
}
