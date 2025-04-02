using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Core.Entities.Projects
{
    public class MissionAssignment
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string MissionId { get; set; } = string.Empty;

        [Required]
        public string ProjectMemberId { get; set; } = string.Empty;

        [ForeignKey(nameof(MissionId))]
        public Mission Mission { get; set; } = null!;

        [ForeignKey(nameof(ProjectMemberId))]
        public ProjectMember ProjectMember { get; set; } = null!;
    }
}
