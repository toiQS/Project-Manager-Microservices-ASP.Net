using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM.Tracking.Domain.Entities
{
    public class ActivityLog
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string UserId { get; set; } = string.Empty;

        public string? ProjectId { get; set; }

        [Required, MaxLength(500)]
        public string Action { get; set; } = string.Empty;

        public DateTime ActionDate { get; set; } = DateTime.UtcNow;
    }
}