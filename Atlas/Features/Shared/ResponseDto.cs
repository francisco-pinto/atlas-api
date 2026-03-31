namespace Atlas.Features.Shared;

public record ResponseDto
{
    public bool Success {get; init;}
}

public record ErrorResponseDto(string ErrorMessage): ResponseDto;
public record SuccessResponseDto(IReadOnlyCollection<Filter> Filters): ResponseDto;
public record Filter(string Name, IReadOnlyCollection<string> Topics);