using ContactManagement.Application.DTOs;
using ContactManagement.Domain.Interfaces;
using ContactManagement.Shared.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContactManagement.Application.Features.Funds.Queries.GetAllFunds
{
    public class GetAllFundsQueryHandler(
        ILogger<GetAllFundsQueryHandler> logger,
        IFundRepository fundRepository) : IRequestHandler<GetAllFundsQuery, Result<IEnumerable<FundDto>>>
    {
        private readonly ILogger<GetAllFundsQueryHandler> _logger = logger;
        private readonly IFundRepository _fundRepository = fundRepository;

        public async Task<Result<IEnumerable<FundDto>>> Handle(GetAllFundsQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting all funds. Include deleted: {IncludeDeleted}", query.IncludeDeleted);

            var fundsResult = await _fundRepository.GetAllAsync(query.IncludeDeleted, cancellationToken);
            if (fundsResult.IsFailure)
            {
                return Result<IEnumerable<FundDto>>.FromResult(fundsResult);
            }

            var funds = fundsResult.Value!;
            var fundDtos = funds.Select(fund => new FundDto(
                fund.Id,
                fund.Name.Value,
                fund.CreatedAt,
                fund.UpdatedAt));

            return Result<IEnumerable<FundDto>>.Success(fundDtos);
        }
    }
}
