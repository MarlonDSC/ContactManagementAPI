using ContactManagement.Application.DTOs;
using ContactManagement.Domain.Entities;
using ContactManagement.Domain.Interfaces;
using ContactManagement.Shared.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContactManagement.Application.Features.Contacts.Commands.CreateContact
{
    public class CreateContactCommandHandler(
        ILogger<CreateContactCommandHandler> logger,
        IContactRepository contactRepository) : IRequestHandler<CreateContactCommand, Result<ContactDto>>
    {
        private readonly ILogger<CreateContactCommandHandler> _logger = logger;
        private readonly IContactRepository _contactRepository = contactRepository;

        public async Task<Result<ContactDto>> Handle(CreateContactCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating contact with name: {Name}", command.Name);

            Result<Contact> contactResult = Contact.Create(
                command.Name,
                command.Email,
                command.PhoneNumber);

            if (contactResult.IsFailure)
            {
                return Result<ContactDto>.FromResult(contactResult);
            }

            var addResult = await _contactRepository.AddAsync(contactResult.Value!, cancellationToken);

            if (addResult.IsFailure)
            {
                return Result<ContactDto>.FromResult(addResult);
            }

            var contact = addResult.Value!;

            var contactDto = new ContactDto(
                contact.Id,
                contact.Name.Value,
                contact.Email?.Value,
                contact.PhoneNumber?.Value,
                contact.CreatedAt,
                contact.UpdatedAt);

            return Result<ContactDto>.Success(contactDto);
        }
    }
}