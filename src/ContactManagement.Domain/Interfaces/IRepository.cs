using ContactManagement.Shared.Common;
using ContactManagement.Shared.Kernel;

namespace ContactManagement.Domain.Interfaces
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        Task<Result<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<Result<TEntity>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<TEntity>>> GetAllAsync(bool includeDeleted = false, CancellationToken cancellationToken = default);
        Task<Result<TEntity>> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<bool>> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<bool>> RestoreAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<bool>> ExistsAsync(Guid id, bool includeDeleted = false, CancellationToken cancellationToken = default);
    }
}
