using ContactManagement.Domain.Errors;
using ContactManagement.Domain.ValueObjects;

namespace ContactManagement.UnitTests.Domain.ValueObjects
{
    public class NameTests
    {
        [Fact]
        public void Create_WithValidName_ShouldSucceed()
        {
            // Arrange
            string validName = "John Doe";

            // Act
            var result = Name.Create(validName);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(validName, result.Value.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithNullOrEmptyName_ShouldFail(string? invalidName)
        {
            // Act
            var result = Name.Create(invalidName);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(DomainErrors.Contact.NameRequired, result.Error);
        }

        [Fact]
        public void Create_WithTooLongName_ShouldFail()
        {
            // Arrange
            string longName = new string('A', 101); // 101 chars

            // Act
            var result = Name.Create(longName);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(DomainErrors.Contact.NameTooLong, result.Error);
        }

        [Fact]
        public void Create_WithMaxLengthName_ShouldSucceed()
        {
            // Arrange
            string maxLengthName = new string('A', 100); // 100 chars

            // Act
            var result = Name.Create(maxLengthName);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(maxLengthName, result.Value.Value);
        }

        [Fact]
        public void Equals_WithSameName_ShouldBeEqual()
        {
            // Arrange
            var name1 = Name.Create("John Doe").Value;
            var name2 = Name.Create("John Doe").Value;

            // Act & Assert
            Assert.Equal(name1, name2);
        }

        [Fact]
        public void Equals_WithDifferentName_ShouldNotBeEqual()
        {
            // Arrange
            var name1 = Name.Create("John Doe").Value;
            var name2 = Name.Create("Jane Doe").Value;

            // Act & Assert
            Assert.NotEqual(name1, name2);
        }
    }
}
