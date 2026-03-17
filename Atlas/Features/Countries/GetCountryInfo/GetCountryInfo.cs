using System.Text.Json;
using Atlas.Features.Shared;
using Atlas.Services;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Features.Countries.GetCountryInfo;

public static partial class GetCountryInfo
{
    private sealed record Request
    {
        [FromRoute(Name = "country-code")] public string CountryCode { get; set; }
        [FromQuery] public IReadOnlyList<string>? Filters { get; set; }
    }


    private sealed class Endpoint(IGeminiApi geminiApi) : EndpointBase
    {
        [HttpGet("/api/v1/countries/{country-code}")]
        public async Task<IActionResult> HandleAsync(Request request, CancellationToken ct)
        {
            //validate country code
            if(!Utils.Utils.CountryCodeIsValid(request.CountryCode.ToUpper()))
            {
                return BadRequest(new ErrorResponseDto($"Invalid country code: {request.CountryCode}"));
            }

            if (!ValidateFilters(request.Filters, out var filtersError))
            {
                return BadRequest(new ErrorResponseDto($"Invalid filters: {filtersError}"));
            }

            var response = await geminiApi.RequestAsync(
                    request.CountryCode, 
                    request.Filters, 
                    ct);

            //TODO: this if is not working. Succes = true or false
            /*TODO: Sometimes the response from AI is duplicated. In general information
            it will return multiple topics sometimes*/
            
            //TODO: Serialization is not correct
            if (response!.Contains("success"))
            {
                return Ok(JsonSerializer.Deserialize<SuccessResponseDto>(response));
            }
            
            return BadRequest(JsonSerializer.Deserialize<ErrorResponseDto>(response));
            
        }
    }
    
    //TODO: ADD THIS TO A MIDDLEWARE
    private static bool ValidateFilters(
        IReadOnlyList<string>? filters, 
        out string? error)
    {
        const int maxFilters = 5;
        const int maxLength = 50;
        var pattern = SpecialCharactersRegex();

        if (filters == null || filters.Count == 0)
        {
            error = null;
            return true;
        }

        var invalids = new List<string>();

        if (filters.Count > maxFilters)
        {
            invalids.Add($"too many filters ({filters.Count} > {maxFilters})");
        }

        for (var i = 0; i < filters.Count; i++)
        {
            var f = filters[i]?.Trim();
            if (string.IsNullOrEmpty(f))
            {
                invalids.Add($"filter[{i}] is empty");
                continue;
            }

            if (f.Length > maxLength)
            {
                invalids.Add($"filter[{i}] exceeds max length {maxLength}");
            }

            if (!pattern.IsMatch(f))
            {
                invalids.Add($"filter[{i}] contains invalid characters");
            }
        }

        if (invalids.Count > 0)
        {
            error = string.Join("; ", invalids);
            return false;
        }

        error = null;
        return true;
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"^[A-Za-z0-9\.\-_]+$", System.Text.RegularExpressions.RegexOptions.Compiled)]
    private static partial System.Text.RegularExpressions.Regex SpecialCharactersRegex();
}