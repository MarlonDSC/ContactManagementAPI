using ContactManagement.Domain.ValueObjects;
using ContactManagement.Shared.Common;
using ContactManagement.Shared.Kernel;

namespace ContactManagement.Domain.Entities
{
    public class Fund : Entity
    {
        public Name Name { get; private set; } = null!;
        
        // Navigation property for FundContacts
        // public ICollection<FundContact> FundContacts { get; private set; }

        private Fund() { }

        public static Result<Fund> Create(string name)
        {
            var nameResult = Name.Create(name);
            if (nameResult.IsFailure)
            {
                return Result<Fund>.FromResult(nameResult);
            }

            var fund = new Fund
            {
                Name = nameResult.Value!
            };

            return Result<Fund>.Success(fund);
        }

        public Result<Fund> Update(string name)
        {
            var nameResult = Name.Create(name);
            if (nameResult.IsFailure)
            {
                return Result<Fund>.FromResult(nameResult);
            }

            Name = nameResult.Value!;
            return Result<Fund>.Success(this);
        }

        public Result<bool> CanDelete()
        {
            // Check if fund has any associated contacts
            // if (FundContacts.Any())
            // {
            //     return Result<bool>.Failure(DomainErrors.Fund.CannotDelete);
            // }

            return Result<bool>.Success(true);
        }
    }
}
