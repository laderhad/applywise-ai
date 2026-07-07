using ApplyWise.Api.Data;
using ApplyWise.Api.Infrastructure;
using ApplyWise.Api.Mappings;
using ApplyWise.Api.Options;
using ApplyWise.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var postgresConnectionString =
    builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException(
        "ConnectionStrings:Postgres is required.");

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(_ => { }, typeof(MappingProfile));
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddDbContext<ApplyWiseDbContext>(options =>
    options.UseNpgsql(postgresConnectionString));
builder.Services.AddScoped<JobMatchService>();
builder.Services.AddScoped<JobMatchHistoryService>();
builder.Services.AddScoped<ResumeUploadService>();
builder.Services.AddSingleton<PdfUploadValidator>();
builder.Services.AddSingleton<PdfTextExtractor>();
builder.Services
    .AddOptions<OllamaOptions>()
    .Bind(builder.Configuration.GetSection(OllamaOptions.SectionName))
    .Validate(
        options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _),
        "Ollama:BaseUrl must be a valid absolute URL.")
    .Validate(
        options => !string.IsNullOrWhiteSpace(options.Model),
        "Ollama:Model is required.")
    .ValidateOnStart();
builder.Services.AddHttpClient<OllamaService>((serviceProvider, httpClient) =>
{
    var options = serviceProvider
        .GetRequiredService<IOptions<OllamaOptions>>()
        .Value;

    httpClient.BaseAddress = new Uri($"{options.BaseUrl.TrimEnd('/')}/");
    httpClient.Timeout = TimeSpan.FromMinutes(5);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();

app.MapControllers();

app.Run();
