using PM.Shared.Dtos;

namespace PM.Shared.Jwt
{
    public interface IJwtService
    {
        public ServiceResult<string> GenerateToken(string userId, string email, string role);
    }
}
