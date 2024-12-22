using Microsoft.EntityFrameworkCore;

namespace Microservice.Appointments.Infrastructure.Repositories.Base;

public abstract class RepositoryBase<TContext, TEntity>(TContext context) where TContext : DbContext where TEntity : class
{
    protected readonly TContext _context = context ?? throw new ArgumentNullException(nameof(context));

    private const string IdPropertyName = "Id";

    /// <summary>
    /// Detaches the tracked entity with the specified ID to avoid conflict during updates.
    /// </summary>
    protected void DetachEntity<TKey>(TKey id)
    {
        var trackedEntity = _context.ChangeTracker.Entries<TEntity>()
            .FirstOrDefault(e => Equals(GetEntityId<TKey>(e.Entity), id));

        if (trackedEntity != null)
            trackedEntity.State = EntityState.Detached;
    }

    /// <summary>
    /// Attaches an entity to the context, detaching any conflicting tracked entity.
    /// </summary>
    protected void AttachEntity(TEntity entity)
    {
        var id = GetEntityId<object>(entity);

        DetachEntity(id);

        _context.Set<TEntity>().Attach(entity);
    }

    /// <summary>
    /// Extracts the "Id" property from the entity using reflection.
    /// </summary>
    private TKey GetEntityId<TKey>(TEntity entity)
    {
        var propertyInfo = entity.GetType().GetProperty(IdPropertyName);
        if (propertyInfo == null)
        {
            throw new InvalidOperationException($"Entity of type {typeof(TEntity).Name} does not have a property named '{IdPropertyName}'.");
        }

        return (TKey)propertyInfo.GetValue(entity)!;
    }
}