using System.Text;

namespace Atlas.PromptTemplates;

public static class PromptTemplate
{
    private static string promptTemplate;

    public static async Task<string> GetPromptAsync(
        string filtersText,
        string countryCode,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(promptTemplate))
        {
            var templatePath = new StringBuilder()
                .Append(AppContext.BaseDirectory)
                .Append("/PromptTemplates/PromptTemplate.txt")
                .ToString();

            promptTemplate = await File.ReadAllTextAsync(
                Path.GetFullPath(templatePath),
                cancellationToken);
        }
        
        return new StringBuilder()
            .Append(promptTemplate)
            .Replace("{{COUNTRY_CODE_ALPHA2}}", countryCode)
            .Replace("{{TOPICS_CSV}}", filtersText)
            .ToString();
    }
}