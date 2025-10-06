using ContactManagement.Application.DTOs;
using ContactManagement.Application.Features.Funds.Queries.GetFund;
using ContactManagement.Domain.Entities;
using ContactManagement.Domain.Errors;
using ContactManagement.Domain.Interfaces;
using ContactManagement.Domain.ValueObjects;
using ContactManagement.Shared.Common;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ContactManagement.UnitTests.Application.Features.Funds.Queries
{
    public class GetFundQueryHandlerTests
    {
        private readonly Mock<ILogger<GetFundQueryHandler>> _mockLogger;
        private readonly Mock<IFundRepository> _mockFundRepository;
        private readonly GetFundQueryHandler _handler;

        public GetFundQueryHandlerTests()
        {
            _mockLogger = new Mock<ILogger<GetFundQueryHandler>>();
            _mockFundRepository = new Mock<IFundRepository>();
            _handler = new GetFundQueryHandler(_mockLogger.Object, _mockFundRepository.Object);
        }

        [Fact]
        public async Task Handle_WithExistingFund_ShouldReturnFund()
        {
            // Arrange
            var fundId = Guid.NewGuid();
            var fund = Fund.Create("Test Fund").Value!;
            // Set the ID property using reflection since it's private
            typeof(Fund).GetProperty("Id")?.GetSetMethod(true)?.Invoke(fund, new object[] { fundId });
            
            var query = new GetFundQuery(fundId);
            
            _mockFundRepository.Setup(repo => repo.GetByIdAsync(fundId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<Fund>.Success(fund));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(fund.Name.Value, result.Value.Name);
            
            _mockFundRepository.Verify(repo => repo.GetByIdAsync(fundId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistingFund_ShouldReturnNotFound()
        {
            // Arrange
            var fundId = Guid.NewGuid();
            var query = new GetFundQuery(fundId);
            
            _mockFundRepository.Setup(repo => repo.GetByIdAsync(fundId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<Fund>.NotFound(DomainErrors.Fund.NotFound));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
            
            _mockFundRepository.Verify(repo => repo.GetByIdAsync(fundId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
