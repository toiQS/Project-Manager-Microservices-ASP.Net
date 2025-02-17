using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using PM.Domain;
using PM.Domain.Models.users;

namespace PM.Infrastructers.Interfaces
{
    public interface IJwtServices
    {
        public ServicesResult<string> GenerateToken(DetailAppUser detailAppUser);
       
        public ServicesResult<DetailAppUser> ParseToken(string token);
    }
}
