using Microsoft.EntityFrameworkCore;
using PM.Identity.Domain.Entities;
using PM.Shared.Dtos;
using PM.Shared.Persistence.Interfaces;

namespace PM.Shared.Persistence.Implements
{
    public class UnitOfWork<TData> : IUnitOfWork<TData> where TData : DbContext
    {
        private readonly TData _context;
        public IRepository<TData, RefreshToken> RefreshTokenRepository { get; }
        public UnitOfWork(TData context)
        {
            _context = context;
            RefreshTokenRepository = new Repository<TData, RefreshToken>(_context);
        }
        public async Task<ServiceResult<bool>> ExecuteTransactionAsync(Func<Task> transactionOperations, CancellationToken cancellationToken = default)
        {
            if (transactionOperations == null)
                return ServiceResult<bool>.Failure("Transaction operations cannot be null.");
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await transactionOperations();
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return ServiceResult<bool>.Failure(ex);
            }
        }
    }

}
