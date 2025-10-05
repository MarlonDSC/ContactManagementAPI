using ContactManagement.Application.Features.Contacts.Commands.CreateContact;
using ContactManagement.Domain.Entities;
using ContactManagement.Domain.Errors;
using ContactManagement.Domain.Interfaces;
using ContactManagement.Domain.ValueObjects;
using ContactManagement.Shared.Common;
using Microsoft.Extensions.Logging;
using Moq;

namespace ContactManagement.UnitTests.Application.Features.Contacts.Commands.CreateContact
{
    public class CreateContactCommandHandlerTests
    {
        private readonly Mock<ILogger<CreateContactCommandHandler>> _mockLogger;
        private readonly Mock<IContactRepository> _mockContactRepository;
        private readonly CreateContactCommandHandler _handler;

        public CreateContactCommandHandlerTests()
        {
            _mockLogger = new Mock<ILogger<CreateContactCommandHandler>>();
            _mockContactRepository = new Mock<IContactRepository>();
            _handler = new CreateContactCommandHandler(_mockLogger.Object, _mockContactRepository.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldCreateContact()
        {
            // Arrange
            var command = new CreateContactCommand("John Doe", "john@example.com", "1234567890");

            // Create a contact and let it generate its own ID
            var contact = Contact.Create(
                "John Doe",
                "john@example.com",
                "1234567890"
            ).Value;

            _mockContactRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<Contact>.Success(contact!));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("John Doe", result.Value.Name);
            Assert.Equal("john@example.com", result.Value.Email);
            Assert.Equal("1234567890", result.Value.PhoneNumber);

            _mockContactRepository.Verify(
                repo => repo.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidName_ShouldReturnFailure()
        {
            // Arrange
            var command = new CreateContactCommand("", "john@example.com", "1234567890");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(DomainErrors.Contact.NameRequired, result.Error);

            _mockContactRepository.Verify(
                repo => repo.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_WithInvalidEmail_ShouldReturnFailure()
        {
            // Arrange
            var command = new CreateContactCommand("John Doe", "invalid-email", "1234567890");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(DomainErrors.Contact.InvalidEmail, result.Error);

            _mockContactRepository.Verify(
                repo => repo.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_WithInvalidPhoneNumber_ShouldReturnFailure()
        {
            // Arrange
            var command = new CreateContactCommand("John Doe", "john@example.com", "123");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(DomainErrors.Contact.InvalidPhoneNumber, result.Error);

            _mockContactRepository.Verify(
                repo => repo.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_WithRepositoryFailure_ShouldReturnFailure()
        {
            // Arrange
            var command = new CreateContactCommand("John Doe", "john@example.com", "1234567890");
            var error = DomainErrors.General.ServerError("Contact", "Server error");

            _mockContactRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<Contact>.BadRequest(error));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(error, result.Error);

            _mockContactRepository.Verify(
                repo => repo.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithNullEmail_ShouldCreateContactWithoutEmail()
        {
            // Arrange
            var command = new CreateContactCommand("John Doe", null, "1234567890");

            // Create a contact and let it generate its own ID
            var contact = Contact.Create(
                "John Doe",
                null,
                "1234567890"
            ).Value;

            _mockContactRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<Contact>.Success(contact!));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("John Doe", result.Value.Name);
            Assert.Null(result.Value.Email);
            Assert.Equal("1234567890", result.Value.PhoneNumber);
        }

        [Fact]
        public async Task Handle_WithNullPhoneNumber_ShouldCreateContactWithoutPhoneNumber()
        {
            // Arrange
            var command = new CreateContactCommand("John Doe", "john@example.com", null);

            // Create a contact and let it generate its own ID
            var contact = Contact.Create(
                "John Doe",
                "john@example.com",
                null
            ).Value;

            _mockContactRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<Contact>.Success(contact!));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("John Doe", result.Value.Name);
            Assert.Equal("john@example.com", result.Value.Email);
            Assert.Null(result.Value.PhoneNumber);
        }
    }
}
