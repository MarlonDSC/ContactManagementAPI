using ContactManagement.Domain.Errors;
using ContactManagement.Domain.ValueObjects;

namespace ContactManagement.UnitTests.Domain.ValueObjects
{
    public class EmailTests
    {
        [Fact]
        public void Create_WithValidEmail_ShouldSucceed()
        {
            // Arrange
            string validEmail = "test@example.com";

            // Act
            var result = Email.Create(validEmail);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(validEmail, result.Value.Value);
        }

        [Fact]
        public void Create_WithNullEmail_ShouldReturnNull()
        {
            // Arrange
            string? nullEmail = null;

            // Act
            var result = Email.Create(nullEmail);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Fact]
        public void Create_WithEmptyEmail_ShouldReturnNull()
        {
            // Arrange
            string emptyEmail = string.Empty;

            // Act
            var result = Email.Create(emptyEmail);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Fact]
        public void Create_WithWhitespaceEmail_ShouldReturnNull()
        {
            // Arrange
            string whitespaceEmail = "   ";

            // Act
            var result = Email.Create(whitespaceEmail);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Value);
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("invalid@")]
        [InlineData("@example.com")]
        [InlineData("invalid@example")]
        [InlineData("invalid@.com")]
        [InlineData("invalid@example..com")]
        public void Create_WithInvalidEmail_ShouldFail(string invalidEmail)
        {
            // Act
            var result = Email.Create(invalidEmail);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(DomainErrors.Contact.InvalidEmail, result.Error);
        }

        [Fact]
        public void Create_WithTooLongEmail_ShouldFail()
        {
            // Arrange
            string longEmail = new string('a', 246) + "@example.com"; // 256 chars total

            // Act
            var result = Email.Create(longEmail);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(DomainErrors.Contact.InvalidEmail, result.Error);
        }

        [Fact]
        public void Equals_WithSameEmail_ShouldBeEqual()
        {
            // Arrange
            var email1 = Email.Create("test@example.com").Value;
            var email2 = Email.Create("test@example.com").Value;

            // Act & Assert
            Assert.Equal(email1, email2);
        }

        [Fact]
        public void Equals_WithDifferentEmail_ShouldNotBeEqual()
        {
            // Arrange
            var email1 = Email.Create("test1@example.com").Value;
            var email2 = Email.Create("test2@example.com").Value;

            // Act & Assert
            Assert.NotEqual(email1, email2);
        }
    }
}
