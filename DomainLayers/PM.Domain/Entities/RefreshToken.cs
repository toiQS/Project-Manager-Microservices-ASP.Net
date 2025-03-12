using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM.Domain.Entities
{
    public class RefreshToken
    {
        [Key]
        public string Id { get; set; }
        public string Token { get; set; } = string.Empty;
        //[ForeignKey(nameof(User))]
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired { get; set; }
        public bool IsRevoke { get; set; }
    }
}
