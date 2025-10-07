using ContactManagement.Application.DTOs;
using ContactManagement.Shared.Common;
using MediatR;

namespace ContactManagement.Application.Features.FundContacts.Queries.GetContactsByFund
{
    public record GetContactsByFundQuery(Guid FundId) : IRequest<Result<IEnumerable<FundContactListItemDto>>>;
}
