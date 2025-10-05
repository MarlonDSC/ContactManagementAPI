using ContactManagement.Domain.Errors;
using ContactManagement.Domain.Interfaces;
using ContactManagement.Infrastructure.Data;
using ContactManagement.Shared.Common;
using ContactManagement.Shared.Kernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ContactManagement.Infrastructure.Repositories
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        protected readonly ApplicationDbContext DbContext;
        protected readonly ILogger<BaseRepository<TEntity>> Logger;
        protected readonly DbSet<TEntity> EntitySet;

        protected BaseRepository(ApplicationDbContext dbContext, ILogger<BaseRepository<TEntity>> logger)
        {
            DbContext = dbContext;
            Logger = logger;
            EntitySet = dbContext.Set<TEntity>();
        }

        public virtual async Task<Result<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                await EntitySet.AddAsync(entity, cancellationToken);
                await DbContext.SaveChangesAsync(cancellationToken);

                Logger.LogInformation("{EntityType} with ID {EntityId} was created successfully", typeof(TEntity).Name, entity.Id);

                return Result<TEntity>.Success(entity);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while adding {EntityType} with ID {EntityId}", typeof(TEntity).Name, entity.Id);
                return Result<TEntity>.InternalServerError(DomainErrors.General.ServerError(typeof(TEntity).Name, ex.Message));
            }
        }

        public virtual async Task<Result<TEntity>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await EntitySet
                    .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);

                if (entity == null)
                {
                    Logger.LogWarning("{EntityType} with ID {EntityId} not found", typeof(TEntity).Name, id);
                    return Result<TEntity>.NotFound(new Error("NotFound", $"{typeof(TEntity).Name} not found"));
                }

                return Result<TEntity>.Success(entity);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while retrieving {EntityType} with ID {EntityId}", typeof(TEntity).Name, id);
                return Result<TEntity>.InternalServerError(DomainErrors.General.ServerError(typeof(TEntity).Name, ex.Message));
            }
        }

        public virtual async Task<Result<IEnumerable<TEntity>>> GetAllAsync(bool includeDeleted = false, CancellationToken cancellationToken = default)
        {
            try
            {
                IQueryable<TEntity> query = EntitySet;
                
                if (!includeDeleted)
                {
                    query = query.Where(e => !e.IsDeleted);
                }
                
                var entities = await query.ToListAsync(cancellationToken);
                return Result<IEnumerable<TEntity>>.Success(entities);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while retrieving all {EntityType} entities", typeof(TEntity).Name);
                return Result<IEnumerable<TEntity>>.InternalServerError(DomainErrors.General.ServerError(typeof(TEntity).Name, ex.Message));
            }
        }

        public virtual async Task<Result<TEntity>> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            try
            {
                entity.UpdateTimestamps();
                EntitySet.Update(entity);
                await DbContext.SaveChangesAsync(cancellationToken);

                Logger.LogInformation("{EntityType} with ID {EntityId} was updated successfully", typeof(TEntity).Name, entity.Id);

                return Result<TEntity>.Success(entity);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while updating {EntityType} with ID {EntityId}", typeof(TEntity).Name, entity.Id);
                return Result<TEntity>.InternalServerError(DomainErrors.General.ServerError(typeof(TEntity).Name, ex.Message));
            }
        }

        public virtual async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entityResult = await GetByIdAsync(id, cancellationToken);
                if (entityResult.IsFailure)
                {
                    // If entity not found, return NotFound result
                    if (entityResult.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return Result<bool>.NotFound(entityResult.Error!);
                    }

                    // Otherwise return the failure with the original error
                    return Result<bool>.BadRequest(entityResult.Error!);
                }

                var entity = entityResult.Value!;

                EntitySet.Remove(entity);
                await DbContext.SaveChangesAsync(cancellationToken);

                Logger.LogInformation("{EntityType} with ID {EntityId} was deleted successfully", typeof(TEntity).Name, id);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while deleting {EntityType} with ID {EntityId}", typeof(TEntity).Name, id);
                return Result<bool>.InternalServerError(DomainErrors.General.ServerError(typeof(TEntity).Name, ex.Message));
            }
        }

        public virtual async Task<Result<bool>> ExistsAsync(Guid id, bool includeDeleted = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = includeDeleted
                    ? EntitySet.Where(e => e.Id == id)
                    : EntitySet.Where(e => e.Id == id && !e.IsDeleted);
                    
                var exists = await query.AnyAsync(cancellationToken);
                return Result<bool>.Success(exists);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while checking if {EntityType} with ID {EntityId} exists", typeof(TEntity).Name, id);
                return Result<bool>.InternalServerError(DomainErrors.General.ServerError(typeof(TEntity).Name, ex.Message));
            }
        }
        
        public virtual async Task<Result<bool>> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                // For soft delete, we need to check if the entity exists, including already soft-deleted ones
                var entity = await EntitySet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
                
                if (entity == null)
                {
                    Logger.LogWarning("{EntityType} with ID {EntityId} not found for soft deletion", typeof(TEntity).Name, id);
                    return Result<bool>.NotFound(new Error("NotFound", $"{typeof(TEntity).Name} not found"));
                }
                
                // If already deleted, return success
                if (entity.IsDeleted)
                {
                    return Result<bool>.Success(true);
                }
                
                // Otherwise, mark as deleted
                entity.SoftDelete();
                await DbContext.SaveChangesAsync(cancellationToken);
                
                Logger.LogInformation("{EntityType} with ID {EntityId} was soft deleted successfully", typeof(TEntity).Name, id);
                
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while soft deleting {EntityType} with ID {EntityId}", typeof(TEntity).Name, id);
                return Result<bool>.InternalServerError(DomainErrors.General.ServerError(typeof(TEntity).Name, ex.Message));
            }
        }
        
        public virtual async Task<Result<bool>> RestoreAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await EntitySet.FirstOrDefaultAsync(e => e.Id == id && e.IsDeleted, cancellationToken);
                
                if (entity == null)
                {
                    return Result<bool>.NotFound(new Error("NotFound", $"{typeof(TEntity).Name} not found or not deleted"));
                }
                
                entity.Restore();
                await DbContext.SaveChangesAsync(cancellationToken);
                
                Logger.LogInformation("{EntityType} with ID {EntityId} was restored successfully", typeof(TEntity).Name, id);
                
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while restoring {EntityType} with ID {EntityId}", typeof(TEntity).Name, id);
                return Result<bool>.InternalServerError(DomainErrors.General.ServerError(typeof(TEntity).Name, ex.Message));
            }
        }
    }
}
