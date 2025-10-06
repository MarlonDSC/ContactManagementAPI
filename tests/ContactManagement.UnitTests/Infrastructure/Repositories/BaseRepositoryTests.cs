using ContactManagement.Domain.Entities;
using ContactManagement.Infrastructure.Data;
using ContactManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace ContactManagement.UnitTests.Infrastructure.Repositories
{
    public class BaseRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly BaseRepository<Contact> _repository;

        public BaseRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;
            var dbContext = new ApplicationDbContext(_dbContextOptions);
            _repository = new ContactRepository(dbContext, Mock.Of<ILogger<ContactRepository>>());
        }

        private ApplicationDbContext CreateDbContext()
        {
            return new ApplicationDbContext(_dbContextOptions);
        }

        private static Contact CreateValidContact()
        {
            var name = "Test Contact";
            var email = "test@example.com";
            var phoneNumber = "1234567890";
            return Contact.Create(name, email, phoneNumber).Value!;
        }

        [Fact]
        public async Task AddAsync_WithValidEntity_ShouldAddToDatabase()
        {
            // Arrange
            using var dbContext = CreateDbContext();
            var contact = CreateValidContact();

            // Act
            var result = await _repository.AddAsync(contact);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(contact.Id, result.Value.Id);

            // Verify in database
            var savedContact = await dbContext.Set<Contact>().FindAsync(contact.Id);
            Assert.NotNull(savedContact);
            Assert.Equal(contact.Id, savedContact.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ShouldReturnEntity()
        {
            // Arrange
            using var dbContext = CreateDbContext();
            var contact = CreateValidContact();
            await dbContext.Set<Contact>().AddAsync(contact);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(contact.Id);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(contact.Id, result.Value.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            using var dbContext = CreateDbContext();
            var nonExistingId = Guid.NewGuid();

            // Act
            var result = await _repository.GetByIdAsync(nonExistingId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetAllAsync_WithMultipleEntities_ShouldReturnAll()
        {
            // Arrange
            using var dbContext = CreateDbContext();

            var contact1 = CreateValidContact();
            var contact2 = CreateValidContact();
            var contact3 = CreateValidContact();

            await dbContext.Set<Contact>().AddRangeAsync(contact1, contact2, contact3);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(3, result.Value.Count());
            Assert.Contains(result.Value, c => c.Id == contact1.Id);
            Assert.Contains(result.Value, c => c.Id == contact2.Id);
            Assert.Contains(result.Value, c => c.Id == contact3.Id);
        }

        [Fact]
        public async Task UpdateAsync_WithExistingEntity_ShouldUpdateDatabase()
        {
            // Arrange
            using var dbContext = CreateDbContext();

            var contact = CreateValidContact();
            await dbContext.Set<Contact>().AddAsync(contact);
            await dbContext.SaveChangesAsync();

            contact.Update("Updated Name", "updated@example.com");

            // Act
            var result = await _repository.UpdateAsync(contact);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);

            // Verify in database
            var updatedContact = await dbContext.Set<Contact>().FindAsync(contact.Id);
            Assert.NotNull(updatedContact);
            Assert.Equal("Updated Name", updatedContact.Name!.Value);
            Assert.Equal("updated@example.com", updatedContact.Email!.Value);
        }

        [Fact]
        public async Task DeleteAsync_WithExistingEntity_ShouldRemoveFromDatabase()
        {
            // Arrange
            using var dbContext = CreateDbContext();

            var contact = CreateValidContact();
            await dbContext.Set<Contact>().AddAsync(contact);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteAsync(contact.Id);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);

            // Verify removed from database - create a new context to ensure we're not using cached entities
            using var verificationContext = CreateDbContext();
            var deletedContact = await verificationContext.Set<Contact>().FindAsync(contact.Id);
            Assert.Null(deletedContact);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            using var dbContext = CreateDbContext();
            var nonExistingId = Guid.NewGuid();

            // Act
            var result = await _repository.DeleteAsync(nonExistingId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task ExistsAsync_WithExistingEntity_ShouldReturnTrue()
        {
            // Arrange
            using var dbContext = CreateDbContext();

            var contact = CreateValidContact();
            await dbContext.Set<Contact>().AddAsync(contact);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(contact.Id);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }

        [Fact]
        public async Task ExistsAsync_WithNonExistingId_ShouldReturnFalse()
        {
            // Arrange
            using var dbContext = CreateDbContext();
            var nonExistingId = Guid.NewGuid();

            // Act
            var result = await _repository.ExistsAsync(nonExistingId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.Value);
        }

        [Fact]
        public async Task SoftDeleteAsync_WithExistingEntity_ShouldMarkAsDeleted()
        {
            // Arrange
            using var dbContext = CreateDbContext();

            var contact = CreateValidContact();
            await dbContext.Set<Contact>().AddAsync(contact);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.SoftDeleteAsync(contact.Id);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);

            // Verify entity is soft deleted
            using var verificationContext = CreateDbContext();
            var deletedContact = await verificationContext.Set<Contact>().FindAsync(contact.Id);
            Assert.NotNull(deletedContact);
            Assert.True(deletedContact.IsDeleted);
            Assert.NotNull(deletedContact.DeletedAt);
        }

        [Fact]
        public async Task SoftDeleteAsync_WithNonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            using var dbContext = CreateDbContext();
            var nonExistingId = Guid.NewGuid();

            // Act
            var result = await _repository.SoftDeleteAsync(nonExistingId);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task RestoreAsync_WithSoftDeletedEntity_ShouldRestoreEntity()
        {
            // Arrange
            using var dbContext = CreateDbContext();

            var contact = CreateValidContact();
            await dbContext.Set<Contact>().AddAsync(contact);
            
            // Manually soft delete the entity
            contact.SoftDelete();
            await dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.RestoreAsync(contact.Id);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);

            // Verify entity is restored
            using var verificationContext = CreateDbContext();
            var restoredContact = await verificationContext.Set<Contact>().FindAsync(contact.Id);
            Assert.NotNull(restoredContact);
            Assert.False(restoredContact.IsDeleted);
            Assert.Null(restoredContact.DeletedAt);
        }

        [Fact]
        public async Task RestoreAsync_WithNonDeletedEntity_ShouldReturnNotFound()
        {
            // Arrange
            using var dbContext = CreateDbContext();

            var contact = CreateValidContact();
            await dbContext.Set<Contact>().AddAsync(contact);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.RestoreAsync(contact.Id);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetAllAsync_WithIncludeDeletedFalse_ShouldExcludeSoftDeletedEntities()
        {
            // Arrange
            using var dbContext = CreateDbContext();

            var contact1 = CreateValidContact();
            var contact2 = CreateValidContact();
            var contact3 = CreateValidContact();

            await dbContext.Set<Contact>().AddRangeAsync(contact1, contact2, contact3);
            
            // Soft delete one contact
            contact2.SoftDelete();
            await dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync(includeDeleted: false);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());
            Assert.Contains(result.Value, c => c.Id == contact1.Id);
            Assert.DoesNotContain(result.Value, c => c.Id == contact2.Id);
            Assert.Contains(result.Value, c => c.Id == contact3.Id);
        }

        [Fact]
        public async Task GetAllAsync_WithIncludeDeletedTrue_ShouldIncludeSoftDeletedEntities()
        {
            // Arrange
            using var dbContext = CreateDbContext();

            var contact1 = CreateValidContact();
            var contact2 = CreateValidContact();
            var contact3 = CreateValidContact();

            await dbContext.Set<Contact>().AddRangeAsync(contact1, contact2, contact3);
            
            // Soft delete one contact
            contact2.SoftDelete();
            await dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync(includeDeleted: true);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(3, result.Value.Count());
            Assert.Contains(result.Value, c => c.Id == contact1.Id);
            Assert.Contains(result.Value, c => c.Id == contact2.Id);
            Assert.Contains(result.Value, c => c.Id == contact3.Id);
        }

        [Fact]
        public async Task ExistsAsync_WithSoftDeletedEntity_AndIncludeDeletedFalse_ShouldReturnFalse()
        {
            // Arrange
            using var dbContext = CreateDbContext();

            var contact = CreateValidContact();
            await dbContext.Set<Contact>().AddAsync(contact);
            
            // Soft delete the contact
            contact.SoftDelete();
            await dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(contact.Id, includeDeleted: false);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.Value);
        }

        [Fact]
        public async Task ExistsAsync_WithSoftDeletedEntity_AndIncludeDeletedTrue_ShouldReturnTrue()
        {
            // Arrange
            using var dbContext = CreateDbContext();

            var contact = CreateValidContact();
            await dbContext.Set<Contact>().AddAsync(contact);
            
            // Soft delete the contact
            contact.SoftDelete();
            await dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(contact.Id, includeDeleted: true);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }
    }
}
