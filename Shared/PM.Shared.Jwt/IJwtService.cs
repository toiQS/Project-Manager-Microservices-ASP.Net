using PM.Shared.Dtos.auths;

namespace PM.Shared.Jwt
{
    public interface IJwtService
    {
        public ServiceResult<string> GenerateToken(string userId, string email, string role);
    }
}
