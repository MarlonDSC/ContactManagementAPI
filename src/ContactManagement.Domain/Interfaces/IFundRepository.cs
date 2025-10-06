using ContactManagement.Domain.Entities;
using ContactManagement.Shared.Common;

namespace ContactManagement.Domain.Interfaces
{
    public interface IFundRepository
    {
        Task<Result<Fund>> AddAsync(Fund entity, CancellationToken cancellationToken = default);
        Task<Result<Fund>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<Fund>>> GetAllAsync(bool includeDeleted = false, CancellationToken cancellationToken = default);
        Task<Result<Fund>> UpdateAsync(Fund entity, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<bool>> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<bool>> RestoreAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<bool>> ExistsAsync(Guid id, bool includeDeleted = false, CancellationToken cancellationToken = default);
        Task<Result<bool>> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
