using ContactManagement.Shared.Common;
using MediatR;

namespace ContactManagement.Application.Features.FundContacts.Commands.RemoveContactFromFund
{
    public record RemoveContactFromFundCommand(Guid ContactId, Guid FundId) : IRequest<Result<bool>>;
}
