using System.Text.Json;
using Atlas.Features.Shared;
using Atlas.PromptTemplates;
using Google.GenAI;
using Google.GenAI.Types;

namespace Atlas.Services;

public interface IGeminiApi
{
    Task<ResponseDto> RequestAsync(
        Utils.Utils.Alpha2Code code,
        string alphaCode,
        IReadOnlyList<string>? filters,
        CancellationToken cancellationToken);

    Task<ResponseDto> RequestAsync(
        Utils.Utils.Alpha2Code code,
        string alphaCode,
        string filter,
        CancellationToken cancellationToken);
}

public class GeminiApi : IGeminiApi
{
    private readonly Client client = new();
    private const string Model = "models/gemma-4-31b-it";

    public async Task<ResponseDto> RequestAsync(
        Utils.Utils.Alpha2Code code,
        string alphaCode,
        IReadOnlyList<string>? filters,
        CancellationToken cancellationToken)
    {
        var filtersText = filters is { Count: > 0 }
            ? $"'{string.Join("','", filters)}'"
            : "general information";

        //TODO: THIS CAN BE CACHED FOREVER
        var prompt = await PromptTemplate.GetPromptAsync(
            code,
            filtersText,
            alphaCode,
            cancellationToken);
        //TODO: ADD A TRY CATCH WITH A GENERIC RETURN IN CASE OF BEING DOWN OR ERROR
        var response = await client.Models.GenerateContentAsync(
            model: Model, //TODO: PUT THIS IN A CONFIG
            contents: prompt,
            cancellationToken: cancellationToken,
            config: new GenerateContentConfig()
            {
                Temperature = 0,
                ResponseMimeType = "application/json",
                ResponseSchema = GeoSuccessResponseSchema
            }
        );

        var responseParts = response.Candidates?.FirstOrDefault()?.Content?.Parts
            ?? throw new InvalidOperationException("The Gemini response did not contain any content parts.");

        var sanitizedResponse = responseParts.ElementAtOrDefault(1)?.Text
            ?? responseParts.ElementAtOrDefault(0)?.Text
            ?? throw new InvalidOperationException("The Gemini response did not contain any text parts.");

        var doc = JsonDocument.Parse(sanitizedResponse);
        var success = doc.RootElement.GetProperty("success").GetBoolean();

        if (success)
        {
            return JsonSerializer.Deserialize<SuccessResponseDto>(sanitizedResponse,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;
        }

        return JsonSerializer.Deserialize<ErrorResponseDto>(sanitizedResponse,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;
    }

    public async Task<ResponseDto> RequestAsync(
        Utils.Utils.Alpha2Code code,
        string alphaCode,
        string filter,
        CancellationToken cancellationToken)
    {
        //TODO: VALIDATE IF WE WANT THIS TO BE LIKE THIS
        var filterText = string.IsNullOrWhiteSpace(filter) ? "general information" : filter;

        //TODO: THIS CAN BE CACHED FOREVER
        var prompt = await PromptTemplate.GetPromptAsync(
            code,
            filterText,
            alphaCode,
            cancellationToken);

        var response = await client.Models.GenerateContentAsync(
            model: "models/gemma-3-27b-it", //TODO: PUT THIS IN A CONFIG
            contents: prompt,
            cancellationToken: cancellationToken
        );

        var responseParts = response.Candidates?.FirstOrDefault()?.Content?.Parts
            ?? throw new InvalidOperationException("The Gemini response did not contain any content parts.");

        var sanitizedResponse = responseParts.ElementAtOrDefault(1)?.Text
            ?? responseParts.ElementAtOrDefault(0)?.Text
            ?? throw new InvalidOperationException("The Gemini response did not contain any text parts.");

        var doc = JsonDocument.Parse(sanitizedResponse);
        var success = doc.RootElement.GetProperty("success").GetBoolean();

        //TODO: VALIDATE DUPLICATED VALUES
        if (success)
        {
            return JsonSerializer.Deserialize<SuccessResponseDto>(sanitizedResponse,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;
        }

        return JsonSerializer.Deserialize<ErrorResponseDto>(sanitizedResponse,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;
    }

    private static readonly Schema GeoSuccessResponseSchema = Schema.FromJson(
        """
        {
          "type": "object",
          "properties": {
            "success": {
              "type": "boolean"
            },
            "filters": {
              "type": "array",
              "items": {
                "type": "object",
                "properties": {
                  "name": {
                    "type": "string",
                    "description": "The original filter name exactly as provided."
                  },
                  "topics": {
                    "type": "array",
                    "minItems": 3,
                    "maxItems": 3,
                    "items": {
                      "type": "string",
                      "description": "A concise factual sentence."
                    }
                  }
                },
                "required": ["name", "topics"]
              }
            }
          },
          "required": ["success", "filters"]
        }
        """
    ) ?? throw new InvalidOperationException("Failed to initialize GeoSuccessResponseSchema.");
}
