using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM.Project.Domain.Entities
{
    public class ProjectMember
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string ProjectId { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string RoleId { get; set; } = string.Empty;

        [MaxLength(255)]
        public string PositionWork { get; set; } = string.Empty;

        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; } = null!;

        [ForeignKey(nameof(RoleId))]
        public RoleInProject RoleInProject { get; set; } = null!;
    }
}
