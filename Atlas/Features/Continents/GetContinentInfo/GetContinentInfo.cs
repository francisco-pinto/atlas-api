using Atlas.Features.Shared;
using Atlas.Services;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Features.Continents.GetContinentInfo;

public static partial class GetContinentInfo
{
    private sealed record Request
    {
        [FromRoute(Name = "continent-code")] public string ContinentCode { get; set; }
        [FromQuery] public IReadOnlyList<string>? Filters { get; set; }
    }


    private sealed class Endpoint(IGeminiApi geminiApi) : EndpointBase
    {
        [HttpGet("/api/v1/continents/{continent-code}")]
        public async Task<IActionResult> HandleAsync(Request request, CancellationToken ct)
        {
            //validate country code
            if(!Utils.Utils.ContinentCodeIsValid(request.ContinentCode.ToUpper()))
            {
                return BadRequest(new ErrorResponseDto($"Invalid continent code: {request.ContinentCode}"));
            }

            if (!FiltersAreValide(request.Filters, out var filtersError))
            {
                return BadRequest(new ErrorResponseDto($"Invalid filters: {filtersError}"));
            }

            var response = await geminiApi.RequestAsync(
                Utils.Utils.Alpha2Code.Continent,
                request.ContinentCode, 
                request.Filters, 
                ct);
          
            if (response.Success)
            {
                return Ok((SuccessResponseDto)response);
            }
            
            return BadRequest((ErrorResponseDto)response);
            
        }
    }
    
    //TODO: ADD THIS TO A MIDDLEWARE
    private static bool FiltersAreValide(
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