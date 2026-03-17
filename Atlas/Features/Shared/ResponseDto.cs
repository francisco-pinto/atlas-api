namespace Atlas.Features.Shared;

public record ResponseDto
{
    public bool Success {get; init;}
}

public record ErrorResponseDto(string ErrorMessage): ResponseDto;
public record SuccessResponseDto(IReadOnlyCollection<Filters> Filters): ResponseDto;
public record Filters(string Name, IReadOnlyCollection<string> Bullets);