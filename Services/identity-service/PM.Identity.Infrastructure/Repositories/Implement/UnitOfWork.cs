using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PM.Identity.Domain.Entities;
using PM.Identity.Infrastructure.Data;
using PM.Identity.Infrastructure.Repositories.Interface;
using PM.Shared.Dtos;

namespace PM.Identity.Infrastructure.Repositories.Implement
{
    public class UnitOfWork
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILoggerFactory _logger;
        private readonly SignInManager<User> _signInManager;
        public IAuthRepository AuthRepository { get; }

        public UnitOfWork(AuthDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            ILoggerFactory logger,
            SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _signInManager = signInManager;
            AuthRepository = new AuthRepository(_userManager, _roleManager, _logger.CreateLogger<AuthRepository>(), _signInManager);
        }

        public async Task<ServiceResult<bool>> ExecuteTransactionAsync(Func<Task<ServiceResult<bool>>> transactionOperations, CancellationToken cancellationToken = default)
        {
            if (transactionOperations == null)
                return ServiceResult<bool>.Failure("Transaction operations cannot be null.");

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var result = await transactionOperations();
                if (!result.Status)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return result;
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return ServiceResult<bool>.Failure("Transaction failed.");
            }
        }
    }
}
