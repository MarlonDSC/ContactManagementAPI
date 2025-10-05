using ContactManagement.Shared.Common;

namespace ContactManagement.Domain.Errors
{
    public static class DomainErrors
    {
        public static class General
        {

            public static Error NotFound(string entityName) => new(
                $"{entityName}.NotFound",
                $"The {entityName} with the specified identifier was not found.");

            public static Error Conflict(string entityName) => new(
                $"{entityName}.Conflict",
                $"The {entityName} with the specified identifier already exists.");
            public static Error ServerError(string entityName, string errorMessage) => new(
                $"{entityName}.ServerError",
                $"An unexpected error occurred while processing the request. {errorMessage}");

            public static readonly Error ValidationError = new(
                "General.ValidationError",
                "One or more validation errors occurred.");

            public static readonly Error Unauthorized = new(
                "General.Unauthorized",
                "You are not authorized to perform this action.");

            public static readonly Error Forbidden = new(
                "General.Forbidden",
                "You do not have permission to perform this action.");
        }
        public static class Contact
        {
            public static Error NotFound => General.NotFound("Contact");

            public static readonly Error NameRequired = new(
                "Contact.NameRequired",
                "The contact name is required.");

            public static readonly Error NameTooLong = new(
                "Contact.NameTooLong",
                "The contact name exceeds the maximum allowed length.");

            public static readonly Error InvalidEmail = new(
                "Contact.InvalidEmail",
                "The provided email address is not valid.");

            public static readonly Error InvalidPhoneNumber = new(
                "Contact.InvalidPhoneNumber",
                "The provided phone number is not valid.");

            public static readonly Error CannotDelete = new(
                "Contact.CannotDelete",
                "The contact cannot be deleted because it is assigned to one or more funds.");

            public static Error AlreadyExists => General.Conflict("Contact");
        }

        public static class Fund
        {
            public static Error NotFound => General.NotFound("Fund");
        }

        public static class FundContact
        {
            public static Error NotFound => General.NotFound("FundContact");

            public static Error AlreadyExists => General.Conflict("FundContact");
        }
    }
}

