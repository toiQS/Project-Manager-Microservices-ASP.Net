using PM.Identity.Infrastructure.Repositories.Implement;
using PM.Shared.Dtos;

namespace PM.Identity.Infrastructure.Repositories.Interface
{
    public interface IUnitOfWork
    {
        public IAuthRepository AuthRepository { get; }


        public Task<ServiceResult<bool>> ExecuteTransactionAsync(Func<ServiceResult<bool>> transactionOperations, CancellationToken cancellationToken = default);
    }
}
