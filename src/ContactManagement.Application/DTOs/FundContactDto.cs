using System;

namespace ContactManagement.Application.DTOs
{
    public record FundContactDto(
        Guid Id,
        Guid ContactId,
        Guid FundId,
        string ContactName,
        string FundName,
        DateTime CreatedAt,
        DateTime? UpdatedAt);

    public record CreateFundContactDto(
        Guid ContactId,
        Guid FundId);

    public record FundContactListItemDto(
        Guid Id,
        string Name,
        string? Email,
        string? PhoneNumber);
}
