using Microsoft.EntityFrameworkCore;

namespace PM.Shared.Infrastructure.Interfaces
{
    public interface IUnitOfWork<TData> where TData : DbContext
    {
        Task ExecuteTransactionAsync(Func<Task> transactionOperations, CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        IRepository<TData, TEntity> Repository<TEntity>() where TEntity : class;
    }
}
