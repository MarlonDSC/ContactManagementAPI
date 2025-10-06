using ContactManagement.Application.DTOs;
using ContactManagement.Application.Features.Funds.Commands.CreateFund;
using ContactManagement.Domain.Entities;
using ContactManagement.Domain.Interfaces;
using ContactManagement.Shared.Common;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ContactManagement.UnitTests.Application.Features.Funds.Commands
{
    public class CreateFundCommandHandlerTests
    {
        private readonly Mock<ILogger<CreateFundCommandHandler>> _mockLogger;
        private readonly Mock<IFundRepository> _mockFundRepository;
        private readonly CreateFundCommandHandler _handler;

        public CreateFundCommandHandlerTests()
        {
            _mockLogger = new Mock<ILogger<CreateFundCommandHandler>>();
            _mockFundRepository = new Mock<IFundRepository>();
            _handler = new CreateFundCommandHandler(_mockLogger.Object, _mockFundRepository.Object);
        }

        [Fact]
        public async Task Handle_WithValidFund_ShouldReturnSuccess()
        {
            // Arrange
            var command = new CreateFundCommand("Test Fund");
            var fund = Fund.Create("Test Fund").Value!;
            
            _mockFundRepository.Setup(repo => repo.ExistsByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<bool>.Success(false));
            
            _mockFundRepository.Setup(repo => repo.AddAsync(It.IsAny<Fund>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<Fund>.Success(fund));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(command.Name, result.Value.Name);
            
            _mockFundRepository.Verify(repo => repo.ExistsByNameAsync(command.Name, It.IsAny<CancellationToken>()), Times.Once);
            _mockFundRepository.Verify(repo => repo.AddAsync(It.IsAny<Fund>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithExistingFundName_ShouldReturnConflict()
        {
            // Arrange
            var command = new CreateFundCommand("Existing Fund");
            
            _mockFundRepository.Setup(repo => repo.ExistsByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<bool>.Success(true));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(System.Net.HttpStatusCode.Conflict, result.StatusCode);
            
            _mockFundRepository.Verify(repo => repo.ExistsByNameAsync(command.Name, It.IsAny<CancellationToken>()), Times.Once);
            _mockFundRepository.Verify(repo => repo.AddAsync(It.IsAny<Fund>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithEmptyFundName_ShouldReturnValidationError()
        {
            // Arrange
            var command = new CreateFundCommand(string.Empty);
            
            // We need to mock the ExistsByNameAsync call since it's called before validation
            _mockFundRepository.Setup(repo => repo.ExistsByNameAsync(string.Empty, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<bool>.Success(false));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
            
            _mockFundRepository.Verify(repo => repo.ExistsByNameAsync(string.Empty, It.IsAny<CancellationToken>()), Times.Once);
            _mockFundRepository.Verify(repo => repo.AddAsync(It.IsAny<Fund>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithRepositoryError_ShouldReturnFailure()
        {
            // Arrange
            var command = new CreateFundCommand("Test Fund");
            var error = new Error("TestError", "Test error message");
            
            _mockFundRepository.Setup(repo => repo.ExistsByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<bool>.BadRequest(error));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(error, result.Error);
            
            _mockFundRepository.Verify(repo => repo.ExistsByNameAsync(command.Name, It.IsAny<CancellationToken>()), Times.Once);
            _mockFundRepository.Verify(repo => repo.AddAsync(It.IsAny<Fund>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
