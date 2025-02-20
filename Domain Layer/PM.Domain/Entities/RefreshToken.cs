using System.ComponentModel.DataAnnotations;

namespace PM.Domain.Entities
{
    public class RefreshToken
    {
        [Key]
        public string Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public bool IsExpired { get; set; }
        public bool IsRevoke { get; set; }
    }
}
