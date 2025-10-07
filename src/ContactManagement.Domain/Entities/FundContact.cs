using ContactManagement.Shared.Common;
using ContactManagement.Shared.Kernel;
using ContactManagement.Domain.Errors;

namespace ContactManagement.Domain.Entities
{
    public class FundContact : Entity
    {
        public Guid ContactId { get; private set; }
        public Guid FundId { get; private set; }

        // Navigation properties
        public Contact Contact { get; private set; } = null!;
        public Fund Fund { get; private set; } = null!;

        private FundContact() { }

        public static Result<FundContact> Create(Guid contactId, Guid fundId)
        {
            if (contactId == Guid.Empty)
            {
                return Result<FundContact>.NotFound(DomainErrors.Contact.NotFound);
            }

            if (fundId == Guid.Empty)
            {
                return Result<FundContact>.NotFound(DomainErrors.Fund.NotFound);
            }

            var fundContact = new FundContact
            {
                ContactId = contactId,
                FundId = fundId
            };

            return Result<FundContact>.Success(fundContact);
        }
    }
}
