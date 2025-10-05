using ContactManagement.Domain.Errors;
using ContactManagement.Shared.Common;
using ContactManagement.Shared.Kernel;

namespace ContactManagement.Domain.ValueObjects
{
    public sealed class Name : ValueObject
    {
        private Name(string value) => Value = value;

        public string Value { get; }

        public static Result<Name> Create(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Result<Name>.BadRequest(DomainErrors.Contact.NameRequired);
            }

            if (value.Length > 100)
            {
                return Result<Name>.BadRequest(DomainErrors.Contact.NameTooLong);
            }

            return Result<Name>.Success(new Name(value));
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}