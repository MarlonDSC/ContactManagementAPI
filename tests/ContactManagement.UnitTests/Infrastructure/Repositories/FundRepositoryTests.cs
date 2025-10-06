using ContactManagement.Domain.Entities;
using ContactManagement.Infrastructure.Data;
using ContactManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ContactManagement.UnitTests.Infrastructure.Repositories
{
    public class FundRepositoryTests
    {
        private readonly Mock<ILogger<FundRepository>> _mockLogger;
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public FundRepositoryTests()
        {
            _mockLogger = new Mock<ILogger<FundRepository>>();
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"FundTestDb_{Guid.NewGuid()}")
                .Options;
        }

        private ApplicationDbContext CreateDbContext()
        {
            return new ApplicationDbContext(_dbContextOptions);
        }

        private static Fund CreateValidFund(string name = "Test Fund")
        {
            return Fund.Create(name).Value!;
        }

        [Fact]
        public async Task ExistsByNameAsync_WithExistingFundName_ShouldReturnTrue()
        {
            // Arrange
            using var dbContext = CreateDbContext();
            var repository = new FundRepository(dbContext, _mockLogger.Object);
            
            var fundName = "Existing Fund";
            var fund = CreateValidFund(fundName);
            await dbContext.Funds.AddAsync(fund);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await repository.ExistsByNameAsync(fundName);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }

        [Fact]
        public async Task ExistsByNameAsync_WithNonExistingFundName_ShouldReturnFalse()
        {
            // Arrange
            using var dbContext = CreateDbContext();
            var repository = new FundRepository(dbContext, _mockLogger.Object);

            // Act
            var result = await repository.ExistsByNameAsync("Non-Existing Fund");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.Value);
        }

        [Fact]
        public async Task ExistsByNameAsync_WithDifferentCaseFundName_ShouldReturnTrue()
        {
            // Arrange
            using var dbContext = CreateDbContext();
            var repository = new FundRepository(dbContext, _mockLogger.Object);
            
            var fundName = "Case Sensitive Fund";
            var fund = CreateValidFund(fundName);
            await dbContext.Funds.AddAsync(fund);
            await dbContext.SaveChangesAsync();

            // Act - Search with different case
            var result = await repository.ExistsByNameAsync("CASE sensitive FUND");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }

        [Fact]
        public async Task ExistsByNameAsync_WithWhitespaceFundName_ShouldHandleTrimming()
        {
            // Arrange
            using var dbContext = CreateDbContext();
            var repository = new FundRepository(dbContext, _mockLogger.Object);
            
            var fundName = "Whitespace Fund";
            var fund = CreateValidFund(fundName);
            await dbContext.Funds.AddAsync(fund);
            await dbContext.SaveChangesAsync();

            // Act - Search with whitespace
            var result = await repository.ExistsByNameAsync("  Whitespace Fund  ");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }

        [Fact]
        public async Task ExistsByNameAsync_WithDeletedFund_ShouldReturnFalse()
        {
            // Arrange
            using var dbContext = CreateDbContext();
            var repository = new FundRepository(dbContext, _mockLogger.Object);
            
            var fundName = "Deleted Fund";
            var fund = CreateValidFund(fundName);
            
            // Mark the fund as deleted
            fund.SoftDelete();
            
            await dbContext.Funds.AddAsync(fund);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await repository.ExistsByNameAsync(fundName);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.Value);
        }
    }
}
