using ContactManagement.Domain.Entities;
using ContactManagement.Domain.Interfaces;
using ContactManagement.Infrastructure.Data;
using ContactManagement.Shared.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ContactManagement.Infrastructure.Repositories
{
    public class FundRepository : BaseRepository<Fund>, IFundRepository
    {
        public FundRepository(ApplicationDbContext dbContext, ILogger<FundRepository> logger) 
            : base(dbContext, logger)
        {
        }

        public async Task<Result<bool>> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            try
            {
                // Trim the input name for comparison
                var trimmedName = name?.Trim() ?? string.Empty;
                
                // First get all non-deleted funds
                var nonDeletedFunds = await EntitySet
                    .Where(f => !f.IsDeleted)
                    .Select(f => new { Fund = f, NameValue = EF.Property<string>(f.Name, "Value") })
                    .ToListAsync(cancellationToken);
                
                // Then perform the case-insensitive and trimmed comparison in memory
                var exists = nonDeletedFunds.Any(f => 
                    string.Equals(
                        f.NameValue.Trim(), 
                        trimmedName, 
                        StringComparison.CurrentCultureIgnoreCase));
                
                return Result<bool>.Success(exists);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while checking if Fund with name {FundName} exists", name);
                return Result<bool>.InternalServerError(
                    Domain.Errors.DomainErrors.General.ServerError(typeof(Fund).Name, ex.Message));
            }
        }
    }
}
