namespace PM.Identity.Domain.Interfaces
{
    public interface IAuthRepository
    {
        public Task<bool> Login(string email, string password);
        public Task<bool> Logout(string email, string password);
        public Task<bool> Register(string email, string username, string password);
    }
}
