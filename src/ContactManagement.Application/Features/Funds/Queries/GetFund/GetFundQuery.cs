using ContactManagement.Application.DTOs;
using ContactManagement.Shared.Common;
using MediatR;

namespace ContactManagement.Application.Features.Funds.Queries.GetFund
{
    public record GetFundQuery(Guid Id) : IRequest<Result<FundDto>>;
}
