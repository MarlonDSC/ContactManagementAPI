using ContactManagement.Domain.ValueObjects;
using ContactManagement.Shared.Common;
using ContactManagement.Shared.Kernel;

namespace ContactManagement.Domain.Entities
{
    public class Contact : Entity
    {
        public Name Name { get; private set; } = null!;
        public Email? Email { get; private set; }
        public PhoneNumber? PhoneNumber { get; private set; }

        // public ICollection<FundContact> FundContacts { get; private set; }

        public static Result<Contact> Create(
            string name,
            string? email = null,
            string? phoneNumber = null)
        {
            var nameResult = Name.Create(name);
            if (nameResult.IsFailure)
            {
                return Result<Contact>.FromResult(nameResult);
            }

            var emailResult = Email.Create(email);
            if (emailResult.IsFailure)
            {
                return Result<Contact>.FromResult(emailResult);
            }

            var phoneNumberResult = PhoneNumber.Create(phoneNumber);
            if (phoneNumberResult.IsFailure)
            {
                return Result<Contact>.FromResult(phoneNumberResult);
            }

            var contact = new Contact()
            {
                Name = nameResult.Value!,
                Email = emailResult.Value!,
                PhoneNumber = phoneNumberResult.Value!,
            };

            return Result<Contact>.Success(contact);
        }

        public Result<Contact> Update(
            string name,
            string? email = null,
            string? phoneNumber = null)
        {
            var nameResult = Name.Create(name);
            if (nameResult.IsFailure)
            {
                return Result<Contact>.FromResult(nameResult);
            }

            var emailResult = Email.Create(email);
            if (emailResult.IsFailure)
            {
                return Result<Contact>.FromResult(emailResult);
            }

            var phoneNumberResult = PhoneNumber.Create(phoneNumber);
            if (phoneNumberResult.IsFailure)
            {
                return Result<Contact>.FromResult(phoneNumberResult);
            }

            Name = nameResult.Value!;
            Email = emailResult.Value!;
            PhoneNumber = phoneNumberResult.Value!;

            return Result<Contact>.Success(this);
        }

        public Result<bool> CanDelete()
        {
            // if (FundContacts.Any())
            // {
            //     return Result<bool>.Failure(DomainErrors.Contact.CannotDelete);
            // }

            return Result<bool>.Success(true);
        }
    }
}

