using PM.Shared.Dtos;

namespace PM.Shared.Handle.Interfaces
{
    public interface IUnitOfWork<TData> where TData : DbContext
    {
        
        public Task<ServiceResult<bool>> ExecuteTransactionAsync(Func<Task> transactionOperations, CancellationToken cancellationToken = default);
    }
}