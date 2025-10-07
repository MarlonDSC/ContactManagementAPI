using ContactManagement.Application.DTOs;
using ContactManagement.Domain.Entities;
using ContactManagement.Domain.Errors;
using ContactManagement.Domain.Interfaces;
using ContactManagement.Shared.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContactManagement.Application.Features.Funds.Commands.CreateMultipleFunds
{
    public class CreateMultipleFundsCommandHandler(
        ILogger<CreateMultipleFundsCommandHandler> logger,
        IFundRepository fundRepository) : IRequestHandler<CreateMultipleFundsCommand, Result<List<FundDto>>>
    {
        private readonly ILogger<CreateMultipleFundsCommandHandler> _logger = logger;
        private readonly IFundRepository _fundRepository = fundRepository;

        public async Task<Result<List<FundDto>>> Handle(CreateMultipleFundsCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating {Count} funds", command.Names.Count);

            if (command.Names.Count == 0)
            {
                _logger.LogWarning("Empty list of fund names provided");
                return Result<List<FundDto>>.ValidationError(DomainErrors.General.ValidationError);
            }

            var fundDtos = new List<FundDto>();

            foreach (var name in command.Names)
            {
                var existsResult = await _fundRepository.ExistsByNameAsync(name, cancellationToken);
                if (existsResult.IsFailure)
                {
                    return Result<List<FundDto>>.FromResult(existsResult);
                }

                if (existsResult.Value)
                {
                    _logger.LogWarning("Fund with name {Name} already exists", name);
                    continue;
                }

                var fundResult = Fund.Create(name);
                if (fundResult.IsFailure)
                {
                    _logger.LogWarning("Failed to create fund with name {Name}: {Error}", name, fundResult.Error?.Message);
                    continue;
                }

                var addResult = await _fundRepository.AddAsync(fundResult.Value!, cancellationToken);
                if (addResult.IsFailure)
                {
                    _logger.LogWarning("Failed to add fund with name {Name}: {Error}", name, addResult.Error?.Message);
                    continue;
                }

                var fund = addResult.Value!;

                var fundDto = new FundDto(
                    fund.Id,
                    fund.Name.Value,
                    fund.CreatedAt,
                    fund.UpdatedAt);

                fundDtos.Add(fundDto);
            }

            if (fundDtos.Count == 0)
            {
                _logger.LogWarning("No funds were created");
                return Result<List<FundDto>>.ValidationError(DomainErrors.General.ValidationError);
            }

            return Result<List<FundDto>>.Success(fundDtos);
        }
    }
}
