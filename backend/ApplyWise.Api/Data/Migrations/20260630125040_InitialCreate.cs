using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplyWise.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "job_match_analyses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    resume_text = table.Column<string>(type: "text", nullable: false),
                    job_description = table.Column<string>(type: "text", nullable: false),
                    match_score = table.Column<int>(type: "integer", nullable: false),
                    strong_points = table.Column<List<string>>(type: "text[]", nullable: false),
                    weak_points = table.Column<List<string>>(type: "text[]", nullable: false),
                    missing_keywords = table.Column<List<string>>(type: "text[]", nullable: false),
                    recommended_bullets = table.Column<List<string>>(type: "text[]", nullable: false),
                    cover_letter_draft = table.Column<string>(type: "text", nullable: false),
                    linkedin_message_draft = table.Column<string>(type: "text", nullable: false),
                    summary = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_job_match_analyses", x => x.id);
                    table.CheckConstraint("ck_job_match_analyses_match_score", "match_score BETWEEN 0 AND 100");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "job_match_analyses");
        }
    }
}
