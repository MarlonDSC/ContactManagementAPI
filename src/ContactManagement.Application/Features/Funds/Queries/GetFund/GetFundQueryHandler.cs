using ContactManagement.Application.DTOs;
using ContactManagement.Domain.Interfaces;
using ContactManagement.Shared.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContactManagement.Application.Features.Funds.Queries.GetFund
{
    public class GetFundQueryHandler(
        ILogger<GetFundQueryHandler> logger,
        IFundRepository fundRepository) : IRequestHandler<GetFundQuery, Result<FundDto>>
    {
        private readonly ILogger<GetFundQueryHandler> _logger = logger;
        private readonly IFundRepository _fundRepository = fundRepository;

        public async Task<Result<FundDto>> Handle(GetFundQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting fund with ID: {Id}", query.Id);

            var fundResult = await _fundRepository.GetByIdAsync(query.Id, cancellationToken);
            if (fundResult.IsFailure)
            {
                return Result<FundDto>.FromResult(fundResult);
            }

            var fund = fundResult.Value!;

            var fundDto = new FundDto(
                fund.Id,
                fund.Name.Value,
                fund.CreatedAt,
                fund.UpdatedAt);

            return Result<FundDto>.Success(fundDto);
        }
    }
}
