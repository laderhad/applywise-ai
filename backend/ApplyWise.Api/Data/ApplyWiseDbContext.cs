using ApplyWise.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplyWise.Api.Data;

public sealed class ApplyWiseDbContext : DbContext
{
    public DbSet<JobMatchAnalysis> JobMatchAnalyses =>
        Set<JobMatchAnalysis>();

    public ApplyWiseDbContext(
        DbContextOptions<ApplyWiseDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var analysis = modelBuilder.Entity<JobMatchAnalysis>();

        analysis.ToTable(
            "job_match_analyses",
            table => table.HasCheckConstraint(
                "ck_job_match_analyses_match_score",
                "match_score BETWEEN 0 AND 100"));

        analysis.HasKey(item => item.Id);

        analysis.Property(item => item.Id)
            .HasColumnName("id");
        analysis.Property(item => item.ResumeText)
            .HasColumnName("resume_text")
            .HasColumnType("text");
        analysis.Property(item => item.JobDescription)
            .HasColumnName("job_description")
            .HasColumnType("text");
        analysis.Property(item => item.MatchScore)
            .HasColumnName("match_score");
        analysis.Property(item => item.StrongPoints)
            .HasColumnName("strong_points")
            .HasColumnType("text[]");
        analysis.Property(item => item.WeakPoints)
            .HasColumnName("weak_points")
            .HasColumnType("text[]");
        analysis.Property(item => item.MissingKeywords)
            .HasColumnName("missing_keywords")
            .HasColumnType("text[]");
        analysis.Property(item => item.RecommendedBullets)
            .HasColumnName("recommended_bullets")
            .HasColumnType("text[]");
        analysis.Property(item => item.CoverLetterDraft)
            .HasColumnName("cover_letter_draft")
            .HasColumnType("text");
        analysis.Property(item => item.LinkedinMessageDraft)
            .HasColumnName("linkedin_message_draft")
            .HasColumnType("text");
        analysis.Property(item => item.Summary)
            .HasColumnName("summary")
            .HasColumnType("text");
        analysis.Property(item => item.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
