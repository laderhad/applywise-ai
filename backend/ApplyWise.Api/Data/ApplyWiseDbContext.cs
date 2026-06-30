using Microsoft.EntityFrameworkCore;

namespace ApplyWise.Api.Data;

public sealed class ApplyWiseDbContext : DbContext
{
    public ApplyWiseDbContext(
        DbContextOptions<ApplyWiseDbContext> options)
        : base(options)
    {
    }
}
