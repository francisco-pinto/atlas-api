using System.Text;

namespace Atlas.PromptTemplates;

public static class PromptTemplate
{
    public static async Task<string> GetCustomPromptAsync(
        Utils.Utils.Alpha2Code code,
        string filter,
        string alpha2Code,
        CancellationToken cancellationToken)
    {
        var promptPath = code is Utils.Utils.Alpha2Code.Continent ? 
            "/PromptTemplates/ContinentPromptTemplate.txt" : 
            "/PromptTemplates/CountryPromptTemplate.txt";
        var placeholder = code is Utils.Utils.Alpha2Code.Continent ? 
            "{{CONTINENT_CODE_ALPHA2}}" : 
            "{{COUNTRY_CODE_ALPHA2}}";
        
        var templatePath = new StringBuilder()
            .Append(AppContext.BaseDirectory)
            .Append(promptPath)
            .ToString();

        var promptTemplate = await File.ReadAllTextAsync(
            Path.GetFullPath(templatePath),
            cancellationToken);
        
        return new StringBuilder()
            .Append(promptTemplate)
            .Replace(placeholder, alpha2Code)
            .Replace("{{TOPICS_CSV}}", filter)
            .ToString();
    }
    
    public static async Task<string> GetPromptAsync(
        Utils.Utils.Alpha2Code code,
        string filtersText,
        string alpha2Code,
        CancellationToken cancellationToken)
    {
        /*TODO: Should we improve the prompt to be more specific?
         It make sense also to only give country info
         Like the request the amount_of_population_vila_real is returning error
         
         Also improve prompt to be more open to requests. Requesting for longevity
         in continents did not worked.*/

        var promptPath = code is Utils.Utils.Alpha2Code.Continent ? 
            "PromptTemplates/ContinentPromptTemplate.txt" : 
            "PromptTemplates/CountryPromptTemplate.txt";
        var continentPlaceholder = code is Utils.Utils.Alpha2Code.Continent ? 
            "{{CONTINENT_CODE_ALPHA2}}" : 
            "{{COUNTRY_CODE_ALPHA2}}";
        var topicPlaceholder = "{{FILTERS_CSV}}";
        
        var templatePath = new StringBuilder()
            .Append(AppContext.BaseDirectory)
            .Append(promptPath)
            .ToString();

        var promptTemplate = await File.ReadAllTextAsync(
            Path.GetFullPath(templatePath),
            cancellationToken);
        
        return new StringBuilder()
            .Append(promptTemplate)
            .Replace(continentPlaceholder, alpha2Code)
            .Replace(topicPlaceholder, filtersText)
            .ToString();
    }
}