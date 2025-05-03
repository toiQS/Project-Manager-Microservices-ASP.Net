using System.ComponentModel.DataAnnotations;

namespace PM.Tracking.Entities
{
    public class ActivityLog
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public string ProjectId { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public DateTime ActionAt { get; set; } = DateTime.UtcNow;
    }
}
