using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM.Domain.Entities
{
    public class MissionAssignment
    {
        [Key]
        public string Id { get; set; } // Mã gán nhiệm vụ
        //[ForeignKey(nameof(Mission))]
        public string MissionId { get; set; } // Mã nhiệm vụ
        //[ForeignKey(nameof(ProjectMember))]
        public string ProjectMemberId { get; set; } // Mã thành viên dự án

        public Mission Mission { get; set; } // Liên kết đến nhiệm vụ
        public ProjectMember ProjectMember { get; set; } // Liên kết đến thành viên dự án
    }
}
