using ContactManagement.Application.DTOs;
using ContactManagement.Domain.Entities;
using ContactManagement.Domain.Errors;
using ContactManagement.Domain.Interfaces;
using ContactManagement.Shared.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContactManagement.Application.Features.FundContacts.Commands.AssignContactToFund
{
    public class AssignContactToFundCommandHandler(
        IFundContactRepository fundContactRepository,
        IContactRepository contactRepository,
        IFundRepository fundRepository,
        ILogger<AssignContactToFundCommandHandler> logger) : IRequestHandler<AssignContactToFundCommand, Result<FundContactDto>>
    {
        private readonly IFundContactRepository _fundContactRepository = fundContactRepository;
        private readonly IContactRepository _contactRepository = contactRepository;
        private readonly IFundRepository _fundRepository = fundRepository;
        private readonly ILogger<AssignContactToFundCommandHandler> _logger = logger;

        public async Task<Result<FundContactDto>> Handle(AssignContactToFundCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Assigning contact {ContactId} to fund {FundId}", request.ContactId, request.FundId);
            
            // Check if contact exists
            var contactResult = await _contactRepository.GetByIdAsync(request.ContactId, cancellationToken);
            if (contactResult.IsFailure)
            {
                return Result<FundContactDto>.FromResult(contactResult);
            }

            // Check if fund exists
            var fundResult = await _fundRepository.GetByIdAsync(request.FundId, cancellationToken);
            if (fundResult.IsFailure)
            {
                return Result<FundContactDto>.FromResult(fundResult);
            }

            // Check if assignment already exists
            var existsResult = await _fundContactRepository.ExistsAsync(request.ContactId, request.FundId);
            if (existsResult.IsFailure)
            {
                return Result<FundContactDto>.FromResult(existsResult);
            }

            if (existsResult.Value)
            {
                _logger.LogWarning("Contact {ContactId} is already assigned to fund {FundId}", request.ContactId, request.FundId);
                return Result<FundContactDto>.Conflict(DomainErrors.FundContact.AlreadyExists);
            }

            // Create the fund contact entity
            var fundContactResult = FundContact.Create(request.ContactId, request.FundId);
            if (fundContactResult.IsFailure)
            {
                return Result<FundContactDto>.FromResult(fundContactResult);
            }

            // Save the entity
            var addResult = await _fundContactRepository.AddAsync(fundContactResult.Value!, cancellationToken);
            if (addResult.IsFailure)
            {
                return Result<FundContactDto>.FromResult(addResult);
            }

            var contact = contactResult.Value!;
            var fund = fundResult.Value!;
            var fundContact = addResult.Value!;

            // Create and return the DTO
            var fundContactDto = new FundContactDto(
                fundContact.Id,
                fundContact.ContactId,
                fundContact.FundId,
                contact.Name.Value,
                fund.Name.Value,
                fundContact.CreatedAt,
                fundContact.UpdatedAt);

            return Result<FundContactDto>.Success(fundContactDto);
        }
    }
}
