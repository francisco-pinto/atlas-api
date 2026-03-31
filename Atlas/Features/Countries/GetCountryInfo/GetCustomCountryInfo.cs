using Atlas.Features.Shared;
using Atlas.Services;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Features.Countries.GetCountryInfo;

public static partial class GetCustomCountryInfo
{
    private sealed record Request
    {
        [FromRoute(Name = "country-code")] public string CountryCode { get; set; }
        [FromQuery] public string Filter { get; set; }
    }


    private sealed class Endpoint(IGeminiApi geminiApi) : EndpointBase
    {
        [HttpGet("/api/v1/countries/{country-code}/custom")]
        public async Task<IActionResult> HandleAsync(Request request, CancellationToken ct)
        {
            //validate country code
            if(!Utils.Utils.CountryCodeIsValid(request.CountryCode.ToUpper()))
            {
                return BadRequest(new ErrorResponseDto($"Invalid country code: {request.CountryCode}"));
            }

            if (!FiltersAreValide(request.Filter, out var filtersError))
            {
                return BadRequest(new ErrorResponseDto($"Invalid filters: {filtersError}"));
            }

            var response = await geminiApi.RequestAsync(
                Utils.Utils.Alpha2Code.Country,
                request.CountryCode, 
                request.Filter, 
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
        string? filter, 
        out string? error)
    {
        const int maxFilters = 5;
        const int maxLength = 100;
        var pattern = SpecialCharactersRegex();
        
        var invalids = new List<string>();
        
        if (string.IsNullOrWhiteSpace(filter))
        {
            error = null;
            return true;
        }
        
        if (filter.Length > maxLength)
        {
            invalids.Add($"filter exceeds max length {filter.Length}");
        }

        if (!pattern.IsMatch(filter.Trim()))
        {
            invalids.Add($"{filter} contains invalid characters");
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