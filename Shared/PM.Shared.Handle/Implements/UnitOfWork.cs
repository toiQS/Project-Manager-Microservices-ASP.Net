using Microsoft.EntityFrameworkCore;
using PM.Shared.Dtos;
using PM.Shared.Handle.Interfaces;

namespace PM.Shared.Handle.Implements
{
    public class UnitOfWork<TData> : IUnitOfWork<TData> where TData : DbContext
    {
        private readonly TData _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(TData context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IRepository<TData, TEntity> Repository<TEntity>() where TEntity : class
        {
            var entityType = typeof(TEntity);
            if (!_repositories.ContainsKey(entityType))
            {
                var repositoryInstance = new Repository<TData, TEntity>(_context);
                _repositories[entityType] = repositoryInstance;
            }

            return (IRepository<TData, TEntity>)_repositories[entityType];
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<ServiceResult<bool>> ExecuteTransactionAsync(Func<Task> transactionOperations, CancellationToken cancellationToken = default)
        {
            if (transactionOperations == null)
                return ServiceResult<bool>.Error("Transaction operations cannot be null.");

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
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
