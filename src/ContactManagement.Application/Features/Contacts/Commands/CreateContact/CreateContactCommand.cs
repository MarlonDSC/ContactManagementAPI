using ContactManagement.Application.DTOs;
using ContactManagement.Shared.Common;
using MediatR;

namespace ContactManagement.Application.Features.Contacts.Commands.CreateContact
{
    public record CreateContactCommand(string Name, string? Email, string? PhoneNumber) : IRequest<Result<ContactDto>>
    {
        public static CreateContactCommand FromDto(CreateContactDto dto)
        {
            return new CreateContactCommand(dto.Name, dto.Email, dto.PhoneNumber);
        }
    }
}
