namespace PM.Shared.Jwt
{
    public interface IJwtService
    {
        public string GenerateToken(string userId, string email, string role);
    }
}
