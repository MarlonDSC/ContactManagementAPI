using ContactManagement.Application.DTOs;
using ContactManagement.Shared.Common;
using MediatR;

namespace ContactManagement.Application.Features.Funds.Commands.CreateMultipleFunds
{
    public record CreateMultipleFundsCommand(List<string> Names) : IRequest<Result<List<FundDto>>>
    {
        public static CreateMultipleFundsCommand FromNames(List<string> names)
        {
            return new CreateMultipleFundsCommand(names);
        }
    }
}
