using ContactManagement.Domain.Entities;
using ContactManagement.Domain.Errors;
using ContactManagement.Domain.Interfaces;
using ContactManagement.Infrastructure.Data;
using ContactManagement.Shared.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ContactManagement.Infrastructure.Repositories
{
    public class FundContactRepository(ApplicationDbContext dbContext, ILogger<FundContactRepository> logger) : BaseRepository<FundContact>(dbContext, logger), IFundContactRepository
    {
        private const string EntityName = "FundContact";

        public async Task<Result<FundContact>> GetByContactAndFundIdAsync(Guid contactId, Guid fundId)
        {
            try
            {
                var fundContact = await EntitySet
                    .FirstOrDefaultAsync(fc => fc.ContactId == contactId && fc.FundId == fundId && !fc.IsDeleted);

                if (fundContact == null)
                {
                    Logger.LogWarning("{EntityName} with ContactId {ContactId} and FundId {FundId} not found", EntityName, contactId, fundId);
                    return Result<FundContact>.NotFound(DomainErrors.FundContact.NotFound);
                }

                return Result<FundContact>.Success(fundContact);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while retrieving {EntityName} with ContactId {ContactId} and FundId {FundId}", EntityName, contactId, fundId);
                return Result<FundContact>.InternalServerError(DomainErrors.General.ServerError(EntityName, ex.Message));
            }
        }

        public async Task<Result<IEnumerable<FundContact>>> GetByFundIdAsync(Guid fundId)
        {
            try
            {
                var fundContacts = await EntitySet
                    .Include(fc => fc.Contact)
                    .Where(fc => fc.FundId == fundId && !fc.IsDeleted)
                    .ToListAsync();

                return Result<IEnumerable<FundContact>>.Success(fundContacts);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while retrieving {EntityName}s for FundId {FundId}", EntityName, fundId);
                return Result<IEnumerable<FundContact>>.InternalServerError(DomainErrors.General.ServerError(EntityName, ex.Message));
            }
        }

        public async Task<Result<IEnumerable<FundContact>>> GetByContactIdAsync(Guid contactId)
        {
            try
            {
                var fundContacts = await EntitySet
                    .Include(fc => fc.Fund)
                    .Where(fc => fc.ContactId == contactId && !fc.IsDeleted)
                    .ToListAsync();

                return Result<IEnumerable<FundContact>>.Success(fundContacts);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while retrieving {EntityName}s for ContactId {ContactId}", EntityName, contactId);
                return Result<IEnumerable<FundContact>>.InternalServerError(DomainErrors.General.ServerError(EntityName, ex.Message));
            }
        }

        public async Task<Result<bool>> ExistsAsync(Guid contactId, Guid fundId)
        {
            try
            {
                var exists = await EntitySet
                    .AnyAsync(fc => fc.ContactId == contactId && fc.FundId == fundId && !fc.IsDeleted);

                return Result<bool>.Success(exists);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while checking if {EntityName} exists with ContactId {ContactId} and FundId {FundId}", EntityName, contactId, fundId);
                return Result<bool>.InternalServerError(DomainErrors.General.ServerError(EntityName, ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(Guid contactId, Guid fundId)
        {
            try
            {
                var fundContactResult = await GetByContactAndFundIdAsync(contactId, fundId);
                if (fundContactResult.IsFailure)
                {
                    return Result<bool>.FromResult(fundContactResult);
                }

                var fundContact = fundContactResult.Value!;

                EntitySet.Remove(fundContact);
                await DbContext.SaveChangesAsync();

                Logger.LogInformation("{EntityName} with ContactId {ContactId} and FundId {FundId} was deleted successfully", EntityName, contactId, fundId);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while deleting {EntityName} with ContactId {ContactId} and FundId {FundId}", EntityName, contactId, fundId);
                return Result<bool>.InternalServerError(DomainErrors.General.ServerError(EntityName, ex.Message));
            }
        }

        public async Task<bool> ContactHasFundAssignmentsAsync(Guid contactId, CancellationToken cancellationToken = default)
        {
            try
            {
                return await EntitySet
                    .AnyAsync(fc => fc.ContactId == contactId && !fc.IsDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while checking if contact {ContactId} has fund assignments", contactId);
                throw new InvalidOperationException($"Failed to check fund assignments for contact {contactId}", ex);
            }
        }
    }
}
