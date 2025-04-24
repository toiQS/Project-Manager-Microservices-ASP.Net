using Microsoft.EntityFrameworkCore;
using PM.Shared.Dtos.auths;

namespace PM.Shared.Handle.Interfaces
{
    public interface IUnitOfWork<TData> where TData : DbContext
    {
        Task<ServiceResult<bool>> ExecuteTransactionAsync(Func<Task> transactionOperations, CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        IRepository<TData, TEntity> Repository<TEntity>() where TEntity : class;
    }
}
