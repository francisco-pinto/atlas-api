using Atlas.Features.Shared;
using Atlas.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Atlas.Features.Curiosities;

public static class GetCuriosityInfo
{
    private sealed record Request
    {
        [FromRoute(Name = "alpha-code")] public Utils.Utils.Alpha2Code Alpha2Code { get; set; }
        [FromRoute(Name = "code")] public string? Code { get; set; }
    }

    private sealed class Endpoint(IGeminiApi geminiApi) : EndpointBase
    {
        [HttpGet("/api/v1/curiosities/{alpha-code}/{code?}")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> HandleAsync(Request request, CancellationToken ct)
        {
            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                if (request.Alpha2Code is Utils.Utils.Alpha2Code.Continent &&
                    !Utils.Utils.ContinentCodeIsValid(request.Code))
                {
                    return BadRequest(new ErrorResponseDto($"Invalid continent: {request.Code}"));
                }

                if (request.Alpha2Code is Utils.Utils.Alpha2Code.Country &&
                    !Utils.Utils.CountryCodeIsValid(request.Code))
                {
                    return BadRequest(new ErrorResponseDto($"Invalid country: {request.Code}"));
                }
            }

            var response = await geminiApi.RequestAsync(
                request.Alpha2Code,
                request.Code,
                ct);

            if (response.Success)
            {
                return Ok((SuccessResponseDto)response);
            }

            return BadRequest((ErrorResponseDto)response);
        }
    }
}