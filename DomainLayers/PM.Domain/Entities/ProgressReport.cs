using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM.Domain.Entities
{
    public class ProgressReport
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string PlanId { get; set; } = string.Empty;

        [Required, MaxLength(2000)]
        public string ReportDetails { get; set; } = string.Empty;

        public DateTime ReportDate { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(PlanId))]
        public Plan Plan { get; set; } = null!;
    }
}
