using ContactManagement.Domain.Entities;
using ContactManagement.Domain.ValueObjects;
using ContactManagement.Infrastructure.Data;
using ContactManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ContactManagement.UnitTests.Infrastructure.Repositories
{
    public class ContactRepositoryTests
    {
        private readonly Mock<ILogger<ContactRepository>> _mockLogger;
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public ContactRepositoryTests()
        {
            _mockLogger = new Mock<ILogger<ContactRepository>>();
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"ContactTestDb_{Guid.NewGuid()}")
                .Options;
        }

        private ApplicationDbContext CreateDbContext()
        {
            return new ApplicationDbContext(_dbContextOptions);
        }

        private static Contact CreateValidContact(string name = "Test Contact", string email = "test@example.com", string phone = "1234567890")
        {
            return Contact.Create(name, email, phone).Value!;
        }

        [Fact]
        public async Task AddAsync_WithValidContact_ShouldAddToDatabase()
        {
            // Arrange
            using var dbContext = CreateDbContext();
            var repository = new ContactRepository(dbContext, _mockLogger.Object);
            var contact = CreateValidContact();

            // Act
            var result = await repository.AddAsync(contact);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);

            // Verify in database
            var savedContact = await dbContext.Set<Contact>().FindAsync(contact.Id);
            Assert.NotNull(savedContact);
            Assert.Equal(contact.Name!.Value, savedContact.Name!.Value);
            Assert.Equal(contact.Email!.Value, savedContact.Email!.Value);
            Assert.Equal(contact.PhoneNumber!.Value, savedContact.PhoneNumber!.Value);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingContact_ShouldReturnContact()
        {
            // Arrange
            using var dbContext = CreateDbContext();
            var repository = new ContactRepository(dbContext, _mockLogger.Object);

            var contact = CreateValidContact();
            await dbContext.Set<Contact>().AddAsync(contact);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(contact.Id);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(contact.Id, result.Value.Id);
            Assert.Equal(contact.Name.Value, result.Value.Name.Value);
        }

        [Fact]
        public async Task GetAllAsync_WithMultipleContacts_ShouldReturnAllContacts()
        {
            // Arrange
            using var dbContext = CreateDbContext();
            var repository = new ContactRepository(dbContext, _mockLogger.Object);

            var contact1 = CreateValidContact("Contact 1", "contact1@example.com");
            var contact2 = CreateValidContact("Contact 2", "contact2@example.com");
            var contact3 = CreateValidContact("Contact 3", "contact3@example.com");

            await dbContext.Set<Contact>().AddRangeAsync(contact1, contact2, contact3);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(3, result.Value.Count());
        }

        [Fact]
        public async Task UpdateAsync_WithValidContact_ShouldUpdateInDatabase()
        {
            // Arrange
            using var dbContext = CreateDbContext();
            var repository = new ContactRepository(dbContext, _mockLogger.Object);

            var contact = CreateValidContact();
            await dbContext.Set<Contact>().AddAsync(contact);
            await dbContext.SaveChangesAsync();

            var newName = Name.Create("Updated Contact").Value!;
            var newEmail = Email.Create("updated@example.com").Value!;
            contact.Update(newName, newEmail);

            // Act
            var result = await repository.UpdateAsync(contact);

            // Assert
            Assert.True(result.IsSuccess);

            // Verify in database
            var updatedContact = await dbContext.Set<Contact>().FindAsync(contact.Id);
            Assert.NotNull(updatedContact);
            Assert.Equal("Updated Contact", updatedContact.Name!.Value);
            Assert.Equal("updated@example.com", updatedContact.Email!.Value);
        }

        [Fact]
        public async Task DeleteAsync_WithExistingContact_ShouldRemoveFromDatabase()
        {
            // Arrange
            using var dbContext = CreateDbContext();
            var repository = new ContactRepository(dbContext, _mockLogger.Object);

            var contact = CreateValidContact();
            await dbContext.Set<Contact>().AddAsync(contact);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await repository.DeleteAsync(contact.Id);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);

            // Verify removed from database
            var deletedContact = await dbContext.Set<Contact>().FindAsync(contact.Id);
            Assert.Null(deletedContact);
        }

        [Fact]
        public async Task ExistsAsync_WithExistingContact_ShouldReturnTrue()
        {
            // Arrange
            using var dbContext = CreateDbContext();
            var repository = new ContactRepository(dbContext, _mockLogger.Object);

            var contact = CreateValidContact();
            await dbContext.Set<Contact>().AddAsync(contact);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await repository.ExistsAsync(contact.Id);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }

        [Fact]
        public async Task ExistsAsync_WithNonExistingContact_ShouldReturnFalse()
        {
            // Arrange
            using var dbContext = CreateDbContext();
            var repository = new ContactRepository(dbContext, _mockLogger.Object);

            // Act
            var result = await repository.ExistsAsync(Guid.NewGuid());

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.Value);
        }
    }
}
