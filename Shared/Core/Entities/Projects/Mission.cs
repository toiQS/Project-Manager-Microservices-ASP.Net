using Shared.Core.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Core.Entities.Projects
{
    public class Mission : AuditableEntity
    {
        public Guid PlanId { get; set; }

        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsCompleted { get; set; } = false;
    }

}
