using Atlas.PromptTemplates;
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

        //TODO: THIS CAN BE CACHED FOREVER
        var prompt = await PromptTemplate.GetPromptAsync(
            filtersText,
            countryCode,
            cancellationToken);
        
        var response = await client.Models.GenerateContentAsync(
            model: "models/gemma-3-27b-it", //TODO: PUT THIS IN A CONFIG
            contents: prompt,
            cancellationToken: cancellationToken
        );

        return response.Candidates![0].Content!.Parts![0].Text!;
    }
}
