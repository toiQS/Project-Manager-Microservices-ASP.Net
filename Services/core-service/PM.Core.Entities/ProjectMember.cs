using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM.Core.Entities
{
    public class ProjectMember
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        
        public string ProjectId { get; set; } = string.Empty;
        public string PositionId { get; set; } = string.Empty;

        public Project Project { get; set; }
        public Position Position { get; set; }    }
}
