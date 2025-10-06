using ContactManagement.Application.DTOs;
using ContactManagement.Application.Features.Funds.Queries.GetAllFunds;
using ContactManagement.Domain.Entities;
using ContactManagement.Domain.Interfaces;
using ContactManagement.Shared.Common;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ContactManagement.UnitTests.Application.Features.Funds.Queries
{
    public class GetAllFundsQueryHandlerTests
    {
        private readonly Mock<ILogger<GetAllFundsQueryHandler>> _mockLogger;
        private readonly Mock<IFundRepository> _mockFundRepository;
        private readonly GetAllFundsQueryHandler _handler;

        public GetAllFundsQueryHandlerTests()
        {
            _mockLogger = new Mock<ILogger<GetAllFundsQueryHandler>>();
            _mockFundRepository = new Mock<IFundRepository>();
            _handler = new GetAllFundsQueryHandler(_mockLogger.Object, _mockFundRepository.Object);
        }

        [Fact]
        public async Task Handle_WithExistingFunds_ShouldReturnAllFunds()
        {
            // Arrange
            var funds = new List<Fund>
            {
                Fund.Create("Fund A").Value!,
                Fund.Create("Fund B").Value!,
                Fund.Create("Fund C").Value!
            };
            
            var query = new GetAllFundsQuery();
            
            _mockFundRepository.Setup(repo => repo.GetAllAsync(false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<IEnumerable<Fund>>.Success(funds));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(funds.Count, result.Value.Count());
            
            _mockFundRepository.Verify(repo => repo.GetAllAsync(false, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNoFunds_ShouldReturnEmptyCollection()
        {
            // Arrange
            var funds = new List<Fund>();
            var query = new GetAllFundsQuery();
            
            _mockFundRepository.Setup(repo => repo.GetAllAsync(false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<IEnumerable<Fund>>.Success(funds));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
            
            _mockFundRepository.Verify(repo => repo.GetAllAsync(false, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithIncludeDeleted_ShouldPassParameterToRepository()
        {
            // Arrange
            var funds = new List<Fund>();
            var query = new GetAllFundsQuery(true);
            
            _mockFundRepository.Setup(repo => repo.GetAllAsync(true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<IEnumerable<Fund>>.Success(funds));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            
            _mockFundRepository.Verify(repo => repo.GetAllAsync(true, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithRepositoryError_ShouldReturnFailure()
        {
            // Arrange
            var error = new Error("TestError", "Test error message");
            var query = new GetAllFundsQuery();
            
            _mockFundRepository.Setup(repo => repo.GetAllAsync(false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<IEnumerable<Fund>>.BadRequest(error));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(error, result.Error);
            
            _mockFundRepository.Verify(repo => repo.GetAllAsync(false, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
