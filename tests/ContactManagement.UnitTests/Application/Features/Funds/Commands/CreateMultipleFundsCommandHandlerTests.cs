using ContactManagement.Application.DTOs;
using ContactManagement.Application.Features.Funds.Commands.CreateMultipleFunds;
using ContactManagement.Domain.Entities;
using ContactManagement.Domain.Interfaces;
using ContactManagement.Shared.Common;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ContactManagement.UnitTests.Application.Features.Funds.Commands
{
    public class CreateMultipleFundsCommandHandlerTests
    {
        private readonly Mock<ILogger<CreateMultipleFundsCommandHandler>> _mockLogger;
        private readonly Mock<IFundRepository> _mockFundRepository;
        private readonly CreateMultipleFundsCommandHandler _handler;

        public CreateMultipleFundsCommandHandlerTests()
        {
            _mockLogger = new Mock<ILogger<CreateMultipleFundsCommandHandler>>();
            _mockFundRepository = new Mock<IFundRepository>();
            _handler = new CreateMultipleFundsCommandHandler(_mockLogger.Object, _mockFundRepository.Object);
        }

        [Fact]
        public async Task Handle_WithValidFundNames_ShouldReturnSuccess()
        {
            // Arrange
            var fundNames = new List<string> { "Fund A", "Fund B", "Fund C" };
            var command = new CreateMultipleFundsCommand(fundNames);
            
            _mockFundRepository.Setup(repo => repo.ExistsByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<bool>.Success(false));
            
            _mockFundRepository.Setup(repo => repo.AddAsync(It.IsAny<Fund>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Fund fund, CancellationToken _) => Result<Fund>.Success(fund));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(fundNames.Count, result.Value.Count);
            
            _mockFundRepository.Verify(repo => repo.ExistsByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(fundNames.Count));
            _mockFundRepository.Verify(repo => repo.AddAsync(It.IsAny<Fund>(), It.IsAny<CancellationToken>()), Times.Exactly(fundNames.Count));
        }

        [Fact]
        public async Task Handle_WithEmptyList_ShouldReturnValidationError()
        {
            // Arrange
            var command = new CreateMultipleFundsCommand(new List<string>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(System.Net.HttpStatusCode.UnprocessableEntity, result.StatusCode);
            
            _mockFundRepository.Verify(repo => repo.ExistsByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockFundRepository.Verify(repo => repo.AddAsync(It.IsAny<Fund>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithSomeExistingFunds_ShouldSkipExistingAndCreateNew()
        {
            // Arrange
            var fundNames = new List<string> { "Existing Fund", "New Fund" };
            var command = new CreateMultipleFundsCommand(fundNames);
            
            _mockFundRepository.Setup(repo => repo.ExistsByNameAsync("Existing Fund", It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<bool>.Success(true));
            
            _mockFundRepository.Setup(repo => repo.ExistsByNameAsync("New Fund", It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<bool>.Success(false));
            
            _mockFundRepository.Setup(repo => repo.AddAsync(It.IsAny<Fund>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Fund fund, CancellationToken _) => Result<Fund>.Success(fund));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Single(result.Value); // Only one fund should be created
            Assert.Equal("New Fund", result.Value[0].Name);
            
            _mockFundRepository.Verify(repo => repo.ExistsByNameAsync("Existing Fund", It.IsAny<CancellationToken>()), Times.Once);
            _mockFundRepository.Verify(repo => repo.ExistsByNameAsync("New Fund", It.IsAny<CancellationToken>()), Times.Once);
            _mockFundRepository.Verify(repo => repo.AddAsync(It.Is<Fund>(f => f.Name.Value == "New Fund"), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithAllInvalidNames_ShouldReturnValidationError()
        {
            // Arrange
            var fundNames = new List<string> { "", "   ", null! };
            var command = new CreateMultipleFundsCommand(fundNames);
            
            // We need to mock the ExistsByNameAsync calls for each name
            foreach (var name in fundNames)
            {
                _mockFundRepository.Setup(repo => repo.ExistsByNameAsync(name, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result<bool>.Success(false));
            }

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(System.Net.HttpStatusCode.UnprocessableEntity, result.StatusCode);
            
            // Verify that ExistsByNameAsync was called for each name
            foreach (var name in fundNames)
            {
                _mockFundRepository.Verify(repo => repo.ExistsByNameAsync(name, It.IsAny<CancellationToken>()), Times.Once);
            }
            
            _mockFundRepository.Verify(repo => repo.AddAsync(It.IsAny<Fund>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
