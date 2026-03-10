using Google.GenAI;

namespace Atlas.Services;

public interface IGeminiApi
{
    Task CustomRequestAsync(string countryCode, IReadOnlyList<string>? filters, CancellationToken cancellationToken);
    Task<string> RequestAsync(string countryCode, IReadOnlyList<string>? filters, CancellationToken cancellationToken);
}

public class GeminiApi: IGeminiApi
{
    private readonly Client client = new();
    
    public async Task CustomRequestAsync(
        string countryCode, 
        IReadOnlyList<string>? filters,
        CancellationToken cancellationToken)
    {
        //var modelList = await client.Models.ListAsync();
        /*var response = await client.Models.GenerateContentAsync(
            model: "models/gemma-3-27b-it", //TODO: PUT THIS IN A CONFIG
            contents: "Explain how AI works in a few words"
        );
        Console.WriteLine(response.Candidates[0].Content.Parts[0].Text);*/
    }
    
    public async Task<string> RequestAsync(
        string countryCode,
        IReadOnlyList<string>? filters, 
        CancellationToken cancellationToken)
    {
        //TODO: VALIDATE IF WE WANT THIS TO BE LIKE THIS
        var filtersText = filters is { Count: > 0 }
            ? string.Join(", ", filters)
            : "general information";

        //TODO: UPDATE AND CLEAN THIS PROMPT. PUT IT IN A CONFIG
        //TODO: OPTIMIZE THE PROMPT TO GET THE BEST RESPONSE WITH THE LEAST TOKENS POSSIBLE
        var contents = $"Give me 3 facts about the country with the country code {countryCode} related to {filtersText}. " +
                       "Keep the answer concise and factual with the least amount of letters possible. " +
                       "If you don't have information about the country or the filters, say 'I don't know'.";
        
        var response = await client.Models.GenerateContentAsync(
            model: "models/gemma-3-27b-it", //TODO: PUT THIS IN A CONFIG
            contents: contents,
            cancellationToken: cancellationToken 
        );

        return response.Candidates![0].Content!.Parts![0].Text!;
    }
}