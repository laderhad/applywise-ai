using ApplyWise.Api.Options;
using ApplyWise.Api.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
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

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
