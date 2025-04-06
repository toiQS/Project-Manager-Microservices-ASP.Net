using PM.Identity.Domain.Entities;

namespace PM.Identity.Application.Interfaces.Services
{
    public interface ITokenService
    {
        public string GenerateAccessToken(User user);
    }
}
