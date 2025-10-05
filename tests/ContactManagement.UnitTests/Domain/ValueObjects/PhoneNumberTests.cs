using ContactManagement.Domain.Errors;
using ContactManagement.Domain.ValueObjects;

namespace ContactManagement.UnitTests.Domain.ValueObjects
{
    public class PhoneNumberTests
    {
        [Theory]
        [InlineData("1234567890")]
        [InlineData("+1 (123) 456-7890")]
        [InlineData("123-456-7890")]
        [InlineData("123.456.7890")]
        [InlineData("(123) 456-7890")]
        [InlineData("123 456 7890")]
        public void Create_WithValidPhoneNumber_ShouldSucceed(string validPhone)
        {
            // Act
            var result = PhoneNumber.Create(validPhone);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(validPhone, result.Value.Value);
        }

        [Fact]
        public void Create_WithNullPhoneNumber_ShouldReturnNull()
        {
            // Arrange
            string? nullPhone = null;

            // Act
            var result = PhoneNumber.Create(nullPhone);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Fact]
        public void Create_WithEmptyPhoneNumber_ShouldReturnNull()
        {
            // Arrange
            string emptyPhone = string.Empty;

            // Act
            var result = PhoneNumber.Create(emptyPhone);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Fact]
        public void Create_WithWhitespacePhoneNumber_ShouldReturnNull()
        {
            // Arrange
            string whitespacePhone = "   ";

            // Act
            var result = PhoneNumber.Create(whitespacePhone);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Theory]
        [InlineData("123")]
        [InlineData("12345")]
        [InlineData("123456789")]
        [InlineData("1234567890123456")]
        [InlineData("abcdefghij")]
        [InlineData("123-abc-4567")]
        public void Create_WithInvalidPhoneNumber_ShouldFail(string invalidPhone)
        {
            // Act
            var result = PhoneNumber.Create(invalidPhone);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(DomainErrors.Contact.InvalidPhoneNumber, result.Error);
        }

        [Fact]
        public void Equals_WithSamePhoneNumber_ShouldBeEqual()
        {
            // Arrange
            var phone1 = PhoneNumber.Create("1234567890").Value;
            var phone2 = PhoneNumber.Create("1234567890").Value;

            // Act & Assert
            Assert.Equal(phone1, phone2);
        }

        [Fact]
        public void Equals_WithDifferentPhoneNumber_ShouldNotBeEqual()
        {
            // Arrange
            var phone1 = PhoneNumber.Create("1234567890").Value;
            var phone2 = PhoneNumber.Create("9876543210").Value;

            // Act & Assert
            Assert.NotEqual(phone1, phone2);
        }

        [Fact]
        public void Create_PreservesOriginalFormat()
        {
            // Arrange
            string formattedPhone = "+1 (123) 456-7890";

            // Act
            var result = PhoneNumber.Create(formattedPhone);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(formattedPhone, result.Value!.Value);
        }
    }
}
