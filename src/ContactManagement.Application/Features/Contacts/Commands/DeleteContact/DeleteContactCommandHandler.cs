using ContactManagement.Domain.Errors;
using ContactManagement.Domain.Interfaces;
using ContactManagement.Shared.Common;
using MediatR;

namespace ContactManagement.Application.Features.Contacts.Commands.DeleteContact
{
    public class DeleteContactCommandHandler : IRequestHandler<DeleteContactCommand, Result<bool>>
    {
        private readonly IContactRepository _contactRepository;
        private readonly IFundContactRepository _fundContactRepository;

        public DeleteContactCommandHandler(
            IContactRepository contactRepository,
            IFundContactRepository fundContactRepository)
        {
            _contactRepository = contactRepository;
            _fundContactRepository = fundContactRepository;
        }

        public async Task<Result<bool>> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
        {
            // Check if contact exists
            var contactResult = await _contactRepository.ExistsAsync(request.Id, false, cancellationToken);
            if (contactResult.IsFailure)
            {
                return Result<bool>.FromResult(contactResult);
            }

            // Check if contact is assigned to any funds
            var hasAssignments = await _fundContactRepository.ContactHasFundAssignmentsAsync(request.Id, cancellationToken);
            if (hasAssignments)
            {
                return Result<bool>.Conflict(DomainErrors.Contact.CannotDelete);
            }

            // Delete the contact
            await _contactRepository.DeleteAsync(request.Id, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
