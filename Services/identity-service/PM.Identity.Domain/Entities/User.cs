using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PM.Identity.Domain.Entities
{
    public class User : IdentityUser
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string AvatarPath { get; set; } = string.Empty;
    }
}
