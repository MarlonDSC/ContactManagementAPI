using System.Text.RegularExpressions;
using ContactManagement.Domain.Errors;
using ContactManagement.Shared.Common;
using ContactManagement.Shared.Kernel;

namespace ContactManagement.Domain.ValueObjects
{
    public sealed partial class PhoneNumber : ValueObject
    {
        private PhoneNumber(string value) => Value = value;

        public string Value { get; }

        public static Result<PhoneNumber> Create(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Result<PhoneNumber>.Success(null!);
            }

            // Remove any non-digit characters for validation
            var digitsOnly = new string([.. value.Where(char.IsDigit)]);

            if (!PhoneRegex().IsMatch(digitsOnly))
            {
                return Result<PhoneNumber>.BadRequest(DomainErrors.Contact.InvalidPhoneNumber);
            }

            // Store the original formatted value
            return Result<PhoneNumber>.Success(new PhoneNumber(value));
        }

        [GeneratedRegex(
            pattern: @"^\d{10,15}$",
            options: RegexOptions.None,
            matchTimeoutMilliseconds: 1000)]
        private static partial Regex PhoneRegex();

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}