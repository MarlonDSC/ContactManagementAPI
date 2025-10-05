namespace ContactManagement.Application.DTOs
{
    public record ContactDto(
        Guid Id,
        string Name,
        string? Email,
        string? PhoneNumber,
        DateTime CreatedAt,
        DateTime? UpdatedAt);

    public record CreateContactDto(
        string Name,
        string? Email,
        string? PhoneNumber);

    public record UpdateContactDto(
        string Name,
        string? Email,
        string? PhoneNumber);
}

