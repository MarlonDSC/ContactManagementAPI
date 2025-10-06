using ContactManagement.Application.DTOs;
using ContactManagement.Shared.Common;
using MediatR;

namespace ContactManagement.Application.Features.Funds.Commands.CreateFund
{
    public record CreateFundCommand(string Name) : IRequest<Result<FundDto>>
    {
        public static CreateFundCommand FromDto(CreateFundDto dto)
        {
            return new CreateFundCommand(dto.Name);
        }
    }
}
