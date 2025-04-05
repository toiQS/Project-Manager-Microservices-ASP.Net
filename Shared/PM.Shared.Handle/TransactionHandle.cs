using PM.Shared.Dtos;

namespace PM.Shared.Handle
{
    public class TransactionHandle
    {
        //public async Task<ServiceResult<bool>> ExecuteTransactionAsync(DbContext,Func<Task<ServiceResult<bool>>> transactionOperations, CancellationToken cancellationToken = default)
        //{
        //    if (transactionOperations == null)
        //        return ServiceResult<bool>.Failure("Transaction operations cannot be null.");

        //    using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        //    try
        //    {
        //        var result = await transactionOperations();
        //        if (!result.Status)
        //        {
        //            await transaction.RollbackAsync(cancellationToken);
        //            return result;
        //        }

        //        await _context.SaveChangesAsync(cancellationToken);
        //        await transaction.CommitAsync(cancellationToken);
        //        return ServiceResult<bool>.Success(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync(cancellationToken);
        //        return ServiceResult<bool>.Failure("Transaction failed.");
        //    }
        //}
    }
}
