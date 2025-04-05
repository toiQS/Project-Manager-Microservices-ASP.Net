using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM.Project.Domain.Entities
{
    public class Mission
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string PlanId { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public DateTime EndDate { get; set; } = DateTime.UtcNow;

        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        public int StatusId { get; set; }

        public bool IsCompleted { get; set; } = false;

        [ForeignKey(nameof(PlanId))]
        public Plan Plan { get; set; } = new Plan();

        [ForeignKey(nameof(StatusId))]
        public Status Status { get; set; } = new Status();
    }
}
