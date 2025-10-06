using ContactManagement.Application.DTOs;
using ContactManagement.Domain.Interfaces;
using ContactManagement.Shared.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContactManagement.Application.Features.Contacts.Commands.UpdateContact
{
    public class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand, Result<ContactDto>>
    {
        private readonly IContactRepository _contactRepository;
        private readonly ILogger<UpdateContactCommandHandler> _logger;

        public UpdateContactCommandHandler(IContactRepository contactRepository, ILogger<UpdateContactCommandHandler> logger)
        {
            _contactRepository = contactRepository;
            _logger = logger;
        }

        public async Task<Result<ContactDto>> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating contact with ID {ContactId}", request.Id);
            var contactResult = await _contactRepository.GetByIdAsync(request.Id, cancellationToken);
            if (contactResult.IsFailure)
            {
                return Result<ContactDto>.FromResult(contactResult);
            }

            var contact = contactResult.Value!;
            var updateResult = contact.Update(
                request.Name,
                request.Email,
                request.PhoneNumber);

            if (updateResult.IsFailure)
            {
                return Result<ContactDto>.FromResult(updateResult);
            }

            var savedResult = await _contactRepository.UpdateAsync(contact, cancellationToken);
            if (savedResult.IsFailure)
            {
                return Result<ContactDto>.FromResult(savedResult);
            }

            var updatedContact = savedResult.Value!;
            var contactDto = new ContactDto(
                updatedContact.Id,
                updatedContact.Name.Value,
                updatedContact.Email?.Value,
                updatedContact.PhoneNumber?.Value,
                updatedContact.CreatedAt,
                updatedContact.UpdatedAt);

            return Result<ContactDto>.Success(contactDto);
        }
    }
}
