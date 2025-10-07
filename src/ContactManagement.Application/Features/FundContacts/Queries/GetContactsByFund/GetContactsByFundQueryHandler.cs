using ContactManagement.Application.DTOs;
using ContactManagement.Domain.Interfaces;
using ContactManagement.Shared.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContactManagement.Application.Features.FundContacts.Queries.GetContactsByFund
{
    public class GetContactsByFundQueryHandler(
        IFundContactRepository fundContactRepository,
        IFundRepository fundRepository,
        ILogger<GetContactsByFundQueryHandler> logger) : IRequestHandler<GetContactsByFundQuery, Result<IEnumerable<FundContactListItemDto>>>
    {
        private readonly IFundContactRepository _fundContactRepository = fundContactRepository;
        private readonly IFundRepository _fundRepository = fundRepository;
        private readonly ILogger<GetContactsByFundQueryHandler> _logger = logger;

        public async Task<Result<IEnumerable<FundContactListItemDto>>> Handle(GetContactsByFundQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting contacts for fund: {FundId}", request.FundId);
            
            // Verify the fund exists
            var fundResult = await _fundRepository.GetByIdAsync(request.FundId, cancellationToken);
            if (fundResult.IsFailure)
            {
                return Result<IEnumerable<FundContactListItemDto>>.FromResult(fundResult);
            }

            // Get all contacts for the fund
            var fundContactsResult = await _fundContactRepository.GetByFundIdAsync(request.FundId);
            if (fundContactsResult.IsFailure)
            {
                return Result<IEnumerable<FundContactListItemDto>>.FromResult(fundContactsResult);
            }

            var fundContacts = fundContactsResult.Value!;

            // Map to DTOs
            var contactDtos = fundContacts.Select(fc => new FundContactListItemDto(
                fc.Contact.Id,
                fc.Contact.Name.Value,
                fc.Contact.Email?.Value,
                fc.Contact.PhoneNumber?.Value));

            return Result<IEnumerable<FundContactListItemDto>>.Success(contactDtos);
        }
    }
}
