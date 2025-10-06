using ContactManagement.Domain.Entities;

namespace ContactManagement.UnitTests.Domain.Entities
{
    public class ContactTests
    {
        private readonly string _validName;
        private readonly string _validEmail;
        private readonly string _validPhoneNumber;

        public ContactTests()
        {
            _validName = "John Doe";
            _validEmail = "john@example.com";
            _validPhoneNumber = "1234567890";
        }

        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            // Act
            var result = Contact.Create(_validName, _validEmail, _validPhoneNumber);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(_validName, result.Value.Name!.Value);
            Assert.Equal(_validEmail, result.Value.Email!.Value);
            Assert.Equal(_validPhoneNumber, result.Value.PhoneNumber!.Value);
        }

        [Fact]
        public void Create_WithNameOnly_ShouldSucceed()
        {
            // Act
            var result = Contact.Create(_validName);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(_validName, result.Value.Name!.Value);
            Assert.Null(result.Value.Email);
            Assert.Null(result.Value.PhoneNumber);
        }

        [Fact]
        public void Create_WithNameAndEmail_ShouldSucceed()
        {
            // Act
            var result = Contact.Create(_validName, _validEmail);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(_validName, result.Value.Name!.Value);
            Assert.Equal(_validEmail, result.Value.Email!.Value);
            Assert.Null(result.Value.PhoneNumber);
        }

        [Fact]
        public void Create_WithNameAndPhoneNumber_ShouldSucceed()
        {
            // Act
            var result = Contact.Create(_validName, null, _validPhoneNumber);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(_validName, result.Value.Name!.Value);
            Assert.Null(result.Value.Email);
            Assert.Equal(_validPhoneNumber, result.Value.PhoneNumber!.Value);
        }

        [Fact]
        public void Update_ShouldUpdateAllProperties()
        {
            // Arrange
            var contact = Contact.Create(_validName).Value;

            // Act
            var result = contact!.Update("Jane Doe", "jane@example.com", "9876543210");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Jane Doe", result.Value!.Name.Value);
            Assert.Equal("jane@example.com", result.Value.Email!.Value);
            Assert.Equal("9876543210", result.Value.PhoneNumber!.Value);
        }

        [Fact]
        public void Update_WithNameOnly_ShouldClearOtherProperties()
        {
            // Arrange
            var contact = Contact.Create(_validName, _validEmail, _validPhoneNumber).Value!;

            // Act
            var result = contact.Update("Jane Doe");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Jane Doe", result.Value!.Name.Value);
            Assert.Null(result.Value.Email);
            Assert.Null(result.Value.PhoneNumber);
        }

        [Fact]
        public void CanDelete_ShouldReturnTrue()
        {
            // Arrange
            var contact = Contact.Create(_validName).Value!;

            // Act
            var result = contact.CanDelete();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }

        [Fact]
        public void Equals_WithSameId_ShouldBeEqual()
        {
            // Arrange
            var contact1 = Contact.Create(_validName).Value;
            var contact2 = contact1; // Same reference

            // Act & Assert
            Assert.Equal(contact1, contact2);
            Assert.True(contact1 == contact2);
        }

        [Fact]
        public void Equals_WithDifferentId_ShouldNotBeEqual()
        {
            // Arrange
            var contact1 = Contact.Create(_validName).Value;
            var contact2 = Contact.Create(_validName).Value;

            // Act & Assert
            Assert.NotEqual(contact1, contact2);
            Assert.True(contact1 != contact2);
        }
    }
}
