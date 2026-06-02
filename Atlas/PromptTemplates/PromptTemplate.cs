using System.Text;

namespace Atlas.PromptTemplates;

public static class PromptTemplate
{
    public static async Task<string> GetCuriositiesPromptAsync(
        Utils.Utils.Alpha2Code code,
        string alpha2Code,
        CancellationToken cancellationToken)
    {
        var promptPath = "";
        var placeholder = "";
        switch (code)
        {
            case Utils.Utils.Alpha2Code.Continent:
                promptPath = "PromptTemplates/ContinentCuriositiesPromptTemplate.txt";
                placeholder = "{{CONTINENT_CODE_ALPHA2}}";
                break;
            case Utils.Utils.Alpha2Code.Country:
                promptPath = "PromptTemplates/CountryCuriositiesPromptTemplate.txt";
                placeholder = "{{COUNTRY_CODE_ALPHA2}}";
                break;
        }
        
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
            .ToString();
    }
    
    public static async Task<string> GetPromptAsync(
        Utils.Utils.Alpha2Code code,
        string filtersText,
        string alpha2Code,
        CancellationToken cancellationToken)
    {
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