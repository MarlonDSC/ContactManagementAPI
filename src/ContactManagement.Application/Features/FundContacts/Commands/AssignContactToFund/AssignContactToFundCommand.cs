using ContactManagement.Application.DTOs;
using ContactManagement.Shared.Common;
using MediatR;

namespace ContactManagement.Application.Features.FundContacts.Commands.AssignContactToFund
{
    public record AssignContactToFundCommand(Guid ContactId, Guid FundId) : IRequest<Result<FundContactDto>>
    {
        public static AssignContactToFundCommand FromDto(CreateFundContactDto dto)
        {
            return new AssignContactToFundCommand(dto.ContactId, dto.FundId);
        }
    }
}
