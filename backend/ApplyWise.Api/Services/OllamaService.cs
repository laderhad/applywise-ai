using System.Net.Http.Json;
using System.Text.Json;
using ApplyWise.Api.Models.Responses;
using ApplyWise.Api.Options;
using ApplyWise.Api.Prompts;
using Microsoft.Extensions.Options;

namespace ApplyWise.Api.Services;

public sealed class OllamaService
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    private static readonly JsonElement AnalysisJsonSchema =
        CreateAnalysisJsonSchema();

    private readonly HttpClient _httpClient;
    private readonly OllamaOptions _options;
    private readonly ILogger<OllamaService> _logger;

    public OllamaService(
        HttpClient httpClient,
        IOptions<OllamaOptions> options,
        ILogger<OllamaService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<AnalyzeJobMatchResponse> AnalyzeAsync(
        string resumeText,
        string jobDescription,
        CancellationToken cancellationToken = default)
    {
        var request = new OllamaGenerateRequest(
            _options.Model,
            JobMatchPromptBuilder.Build(resumeText, jobDescription),
            Stream: false,
            AnalysisJsonSchema,
            new OllamaGenerateOptions(Temperature: 0));

        try
        {
            using var response = await _httpClient.PostAsJsonAsync(
                "api/generate",
                request,
                JsonOptions,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(
                    cancellationToken);

                _logger.LogWarning(
                    "Ollama returned status {StatusCode}: {ErrorBody}",
                    (int)response.StatusCode,
                    errorBody);

                throw new OllamaServiceException(
                    $"Ollama returned HTTP {(int)response.StatusCode}.");
            }

            var generateResponse =
                await response.Content.ReadFromJsonAsync<OllamaGenerateResponse>(
                    JsonOptions,
                    cancellationToken);

            if (string.IsNullOrWhiteSpace(generateResponse?.Response))
            {
                throw new OllamaServiceException(
                    "Ollama returned an empty response.");
            }

            var result = JsonSerializer.Deserialize<AnalyzeJobMatchResponse>(
                generateResponse.Response,
                JsonOptions);

            ValidateResult(result);
            return result!;
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (OllamaServiceException)
        {
            throw;
        }
        catch (TaskCanceledException exception)
        {
            throw new OllamaServiceException(
                "Ollama did not respond before the request timed out.",
                exception);
        }
        catch (HttpRequestException exception)
        {
            throw new OllamaServiceException(
                "Could not connect to Ollama. Make sure it is running locally.",
                exception);
        }
        catch (JsonException exception)
        {
            _logger.LogWarning(
                exception,
                "Ollama response JSON could not be parsed at path {JsonPath}.",
                exception.Path);

            throw new OllamaServiceException(
                "Ollama returned JSON in an unexpected format.",
                exception);
        }
    }

    private static void ValidateResult(AnalyzeJobMatchResponse? result)
    {
        if (result is null)
        {
            throw new OllamaServiceException(
                "Ollama returned an empty analysis result.");
        }

        if (result.MatchScore is < 0 or > 100)
        {
            throw new OllamaServiceException(
                "Ollama returned a match score outside the 0-100 range.");
        }

        if (result.StrongPoints is null ||
            result.WeakPoints is null ||
            result.MissingKeywords is null ||
            result.RecommendedBullets is null ||
            result.CoverLetterDraft is null ||
            result.LinkedinMessageDraft is null ||
            result.Summary is null)
        {
            throw new OllamaServiceException(
                "Ollama omitted one or more required analysis fields.");
        }
    }

    private static JsonElement CreateAnalysisJsonSchema()
    {
        return JsonSerializer.SerializeToElement(
            new
            {
                type = "object",
                properties = new
                {
                    matchScore = new
                    {
                        type = "integer",
                        minimum = 0,
                        maximum = 100
                    },
                    strongPoints = new
                    {
                        type = "array",
                        items = new { type = "string" }
                    },
                    weakPoints = new
                    {
                        type = "array",
                        items = new { type = "string" }
                    },
                    missingKeywords = new
                    {
                        type = "array",
                        items = new { type = "string" }
                    },
                    recommendedBullets = new
                    {
                        type = "array",
                        items = new { type = "string" }
                    },
                    coverLetterDraft = new { type = "string" },
                    linkedinMessageDraft = new { type = "string" },
                    summary = new { type = "string" }
                },
                required = new[]
                {
                    "matchScore",
                    "strongPoints",
                    "weakPoints",
                    "missingKeywords",
                    "recommendedBullets",
                    "coverLetterDraft",
                    "linkedinMessageDraft",
                    "summary"
                },
                additionalProperties = false
            },
            JsonOptions);
    }

    private sealed record OllamaGenerateRequest(
        string Model,
        string Prompt,
        bool Stream,
        JsonElement Format,
        OllamaGenerateOptions Options);

    private sealed record OllamaGenerateOptions(double Temperature);

    private sealed class OllamaGenerateResponse
    {
        public string Response { get; init; } = string.Empty;
    }
}
