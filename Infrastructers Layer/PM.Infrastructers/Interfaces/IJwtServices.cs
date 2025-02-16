using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using PM.Domain;

namespace PM.Infrastructers.Interfaces
{
    public interface IJwtServices
    {
        public ServicesResult<string> GenerateToken();
        public ServicesResult<string> RefreshToken(string token);
        public ServicesResult<string> JwtExpirationHanding(string token);
        public ServicesResult<string> CheckToken(string token);
        public ServicesResult<string> CancelToken(string token);
        public ServicesResult<string> HandleToken(string token);
    }
}
