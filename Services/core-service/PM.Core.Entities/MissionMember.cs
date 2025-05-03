using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM.Core.Entities
{
    public class MissionMember
    {
        [Key]
        public string Id { get; set; } = string.Empty;
       
        public string MemberId { get; set; } = string.Empty;
        public string MissionId { get; set; } = string.Empty;

        public ProjectMember ProjectMember { get; set; }
        public Mission Mission { get; set; }

    }
}
