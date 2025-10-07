using ContactManagement.Domain.Entities;
using ContactManagement.Shared.Common;

namespace ContactManagement.Domain.Interfaces
{
    public interface IFundContactRepository : IRepository<FundContact>
    {
        Task<Result<FundContact>> GetByContactAndFundIdAsync(Guid contactId, Guid fundId);
        Task<Result<IEnumerable<FundContact>>> GetByFundIdAsync(Guid fundId);
        Task<Result<IEnumerable<FundContact>>> GetByContactIdAsync(Guid contactId);
        Task<Result<bool>> ExistsAsync(Guid contactId, Guid fundId);
        Task<Result<bool>> DeleteAsync(Guid contactId, Guid fundId);
        Task<bool> ContactHasFundAssignmentsAsync(Guid contactId, CancellationToken cancellationToken = default);
    }
}
