using System.ComponentModel.DataAnnotations;

namespace PM.Tracking.Domain
{
    public class ActivityLog
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Action {  get; set; } = string.Empty;
        public string ProjectId {  get; set; } = string.Empty;
        public string UserId {  get; set; } = string.Empty;
        public DateTime ActionAt { get; set; }
    }
}
