using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM.Domain.Entities
{
    public class ProjectMember
    {
        [Key]
        public string Id { get; set; } // Mã thành viên dự án
        [ForeignKey(nameof(Project))]
        public string ProjectId { get; set; } // Mã dự án
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } // Mã người dùng
        [ForeignKey(nameof(RoleInProject))]
        public string RoleId { get; set; } // Vai trò trong dự án
        public string PositionWork { get; set; } = string.Empty;

        public Project Project { get; set; } // Liên kết đến dự án
        public User User { get; set; } // Liên kết đến người dùng
        public RoleInProject RoleInProject { get; set; } // Liên kết đến vai trò
    }
}
