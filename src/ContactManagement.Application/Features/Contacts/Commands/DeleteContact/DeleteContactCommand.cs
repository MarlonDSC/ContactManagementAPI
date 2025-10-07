using ContactManagement.Shared.Common;
using MediatR;

namespace ContactManagement.Application.Features.Contacts.Commands.DeleteContact
{
    public record DeleteContactCommand(Guid Id) : IRequest<Result<bool>>;
}
