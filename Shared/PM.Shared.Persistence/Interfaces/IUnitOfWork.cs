using Microsoft.EntityFrameworkCore;
using PM.Identity.Domain.Entities;
using PM.Shared.Dtos;

namespace PM.Shared.Persistence.Interfaces
{
    public interface IUnitOfWork<TData> where TData : DbContext
    {
        public IRepository<TData, RefreshToken> RefreshTokenRepository { get; }
        public Task<ServiceResult<bool>> ExecuteTransactionAsync(Func<Task> transactionOperations, CancellationToken cancellationToken = default);
    }
}
