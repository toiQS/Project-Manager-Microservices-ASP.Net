using Microsoft.EntityFrameworkCore;
using PM.Shared.Dtos;
using PM.Shared.Handle.Interfaces;

namespace PM.Shared.Handle.Implements
{
    public class UnitOfWork<TData> : IUnitOfWork<TData> where TData : DbContext
    {
        private readonly TData _context;
       
        public UnitOfWork(TData context)
        {
            _context = context;
          
        }
        public async Task<ServiceResult<bool>> ExecuteTransactionAsync(Func<Task> transactionOperations, CancellationToken cancellationToken = default)
        {
            if (transactionOperations == null)
                return ServiceResult<bool>.Error("Transaction operations cannot be null.");
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
                return ServiceResult<bool>.FromException(ex);
            }
        }
    }

}