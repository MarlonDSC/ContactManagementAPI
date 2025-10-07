using ContactManagement.Domain.Interfaces;
using ContactManagement.Shared.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContactManagement.Application.Features.FundContacts.Commands.RemoveContactFromFund
{
    public class RemoveContactFromFundCommandHandler(
        IFundContactRepository fundContactRepository,
        ILogger<RemoveContactFromFundCommandHandler> logger) : IRequestHandler<RemoveContactFromFundCommand, Result<bool>>
    {
        private readonly IFundContactRepository _fundContactRepository = fundContactRepository;
        private readonly ILogger<RemoveContactFromFundCommandHandler> _logger = logger;

        public async Task<Result<bool>> Handle(RemoveContactFromFundCommand request, CancellationToken cancellationToken)
        {
            // Check if the assignment exists
            var fundContactResult = await _fundContactRepository.GetByContactAndFundIdAsync(request.ContactId, request.FundId);
            if (fundContactResult.IsFailure)
            {
                return Result<bool>.FromResult(fundContactResult);
            }

            // Delete the assignment
            var deleteResult = await _fundContactRepository.DeleteAsync(request.ContactId, request.FundId);
            if (deleteResult.IsFailure)
            {
                return Result<bool>.FromResult(deleteResult);
            }

            _logger.LogInformation("Contact {ContactId} was successfully removed from fund {FundId}", request.ContactId, request.FundId);
            return Result<bool>.Success(true);
        }
    }
}
