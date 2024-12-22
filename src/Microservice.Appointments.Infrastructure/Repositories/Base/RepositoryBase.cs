using Microsoft.EntityFrameworkCore;

namespace Microservice.Appointments.Infrastructure.Repositories.Base;
public abstract class RepositoryBase<TContext, TEntity>(TContext context) where TContext : DbContext where TEntity : class
{
    protected readonly TContext _context = context ?? throw new ArgumentNullException(nameof(context));

    /// <summary>
    /// Detaches the tracked entity with the specified ID to avoid conflict during updates.
    /// </summary>
    protected void DetachEntity(int id)
    {
        var trackedEntity = _context.ChangeTracker.Entries<TEntity>()
            .FirstOrDefault(e => EF.Property<int>(e.Entity, "Id") == id);

        if (trackedEntity != null)
            trackedEntity.State = EntityState.Detached;
    }
}