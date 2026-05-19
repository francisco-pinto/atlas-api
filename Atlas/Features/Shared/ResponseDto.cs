namespace Atlas.Features.Shared;

public abstract record ResponseDto(bool Success);

public sealed record ErrorResponseDto(string ErrorMessage)
    : ResponseDto(false);

public sealed record SuccessResponseDto(IReadOnlyCollection<Filter> Filters)
    : ResponseDto(true);

public sealed record Filter(
    string Name,
    IReadOnlyCollection<string> Topics
);