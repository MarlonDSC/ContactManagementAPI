using ContactManagement.Domain.Entities;
using ContactManagement.Shared.Common;

namespace ContactManagement.Domain.Interfaces
{
    public interface IContactRepository
    {
        Task<Result<Contact>> AddAsync(Contact entity, CancellationToken cancellationToken = default);
        Task<Result<Contact>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<Contact>>> GetAllAsync(bool includeDeleted = false, CancellationToken cancellationToken = default);
        Task<Result<Contact>> UpdateAsync(Contact entity, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<bool>> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<bool>> RestoreAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<bool>> ExistsAsync(Guid id, bool includeDeleted = false, CancellationToken cancellationToken = default);
    }
}
