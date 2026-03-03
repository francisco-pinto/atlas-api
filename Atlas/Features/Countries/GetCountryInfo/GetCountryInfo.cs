using Atlas.Features.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Features.Countries.GetCountryInfo;

public static class GetCountryInfo
{
    private sealed record Request
    {
        [FromRoute(Name = "country-code")] public string CountryCode { get; set; }
    }

    
    private sealed class Endpoint : EndpointBase
    {
        [HttpGet("/api/v1/countries/{country-code}/filters")]
        public async Task<IActionResult> HandleAsync(Request request, CancellationToken ct)
        { 
            return NoContent();
        }
    }
}