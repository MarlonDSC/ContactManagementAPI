using ContactManagement.Application.DTOs;
using ContactManagement.Shared.Common;
using MediatR;

namespace ContactManagement.Application.Features.Contacts.Commands.UpdateContact
{
    public record UpdateContactCommand(Guid Id, string Name, string? Email, string? PhoneNumber) : IRequest<Result<ContactDto>>
    {
        public static UpdateContactCommand FromDto(Guid id, UpdateContactDto dto)
        {
            return new UpdateContactCommand(id, dto.Name, dto.Email, dto.PhoneNumber);
        }
    }
}
