using PM.Domain.Entities;

namespace PM.Domain.Interfaces
{
    public interface IAuthUnitOfWork
    {
        IQueryRepository<ActivityLog, string> ActivityLogQueryRepository { get; }
        IQueryRepository<RefreshToken, string> RefreshTokenQueryRepoitory { get; } 
        IQueryRepository<User, string> UserQueryRepository { get; }
        ICommandRepository<ActivityLog, string> ActivityLogCommandRepository { get; }
        ICommandRepository<RefreshToken, string> RefreshTokenCommandRepoitory { get; }
        ICommandRepository<User, string> UserCommandRepository { get; }
        
    }
}
