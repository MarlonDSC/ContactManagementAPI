using ContactManagement.Application.DTOs;
using ContactManagement.Domain.Entities;
using ContactManagement.Domain.Interfaces;
using ContactManagement.Shared.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContactManagement.Application.Features.Funds.Commands.CreateFund
{
    public class CreateFundCommandHandler(
        ILogger<CreateFundCommandHandler> logger,
        IFundRepository fundRepository) : IRequestHandler<CreateFundCommand, Result<FundDto>>
    {
        private readonly ILogger<CreateFundCommandHandler> _logger = logger;
        private readonly IFundRepository _fundRepository = fundRepository;

        public async Task<Result<FundDto>> Handle(CreateFundCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating fund with name: {Name}", command.Name);

            var existsResult = await _fundRepository.ExistsByNameAsync(command.Name, cancellationToken);
            if (existsResult.IsFailure)
            {
                return Result<FundDto>.FromResult(existsResult);
            }

            if (existsResult.Value)
            {
                _logger.LogWarning("Fund with name {Name} already exists", command.Name);
                return Result<FundDto>.Conflict(Domain.Errors.DomainErrors.Fund.AlreadyExists);
            }

            var fundResult = Fund.Create(command.Name);
            if (fundResult.IsFailure)
            {
                return Result<FundDto>.FromResult(fundResult);
            }

            var addResult = await _fundRepository.AddAsync(fundResult.Value!, cancellationToken);
            if (addResult.IsFailure)
            {
                return Result<FundDto>.FromResult(addResult);
            }

            var fund = addResult.Value!;

            var fundDto = new FundDto(
                fund.Id,
                fund.Name.Value,
                fund.CreatedAt,
                fund.UpdatedAt);

            return Result<FundDto>.Success(fundDto);
        }
    }
}
