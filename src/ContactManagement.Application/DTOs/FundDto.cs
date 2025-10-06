namespace ContactManagement.Application.DTOs
{
    public record FundDto(
        Guid Id,
        string Name,
        DateTime CreatedAt,
        DateTime UpdatedAt);

    public record CreateFundDto(
        string Name);

    public record UpdateFundDto(
        string Name);
}
