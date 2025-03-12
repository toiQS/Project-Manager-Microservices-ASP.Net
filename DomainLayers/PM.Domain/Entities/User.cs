using Microsoft.AspNetCore.Identity;

namespace PM.Domain.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public string Email { get; set; }
        public string FullName { get; set; }

        public string AvatarPath { get; set; } // Đường dẫn ảnh đại diện
    }
}
