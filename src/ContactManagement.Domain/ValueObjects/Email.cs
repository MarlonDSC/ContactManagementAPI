using System.Text.RegularExpressions;
using ContactManagement.Domain.Errors;
using ContactManagement.Shared.Common;
using ContactManagement.Shared.Kernel;

namespace ContactManagement.Domain.ValueObjects
{
    public sealed partial class Email : ValueObject
    {
        private Email(string value) => Value = value;

        public string Value { get; }

        public static Result<Email> Create(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Result<Email>.Success(null!);
            }

            if (!EmailRegex().IsMatch(value))
            {
                return Result<Email>.BadRequest(DomainErrors.Contact.InvalidEmail);
            }

            if (value.Length > 255)
            {
                return Result<Email>.BadRequest(DomainErrors.Contact.InvalidEmail);
            }

            return Result<Email>.Success(new Email(value));
        }

        [GeneratedRegex(
            pattern: @"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$",
            options: RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture,
            matchTimeoutMilliseconds: 1000)]
        private static partial Regex EmailRegex();

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}