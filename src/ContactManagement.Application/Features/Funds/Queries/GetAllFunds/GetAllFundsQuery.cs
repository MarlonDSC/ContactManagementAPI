using ContactManagement.Application.DTOs;
using ContactManagement.Shared.Common;
using MediatR;

namespace ContactManagement.Application.Features.Funds.Queries.GetAllFunds
{
    public record GetAllFundsQuery(bool IncludeDeleted = false) : IRequest<Result<IEnumerable<FundDto>>>;
}
