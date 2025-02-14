using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM.Domain.Entities
{
    public class ActivityLog
    {
        [Key]
        public string Id { get; set; } // Mã nhật ký
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } // Mã người dùng
        [ForeignKey(nameof(Project))]
        public string? ProjectId { get; set; } // Mã dự án
        public string Action { get; set; } // Hành động thực hiện
        public DateTime ActionDate { get; set; } // Ngày thực hiện

        public User User { get; set; } // Liên kết đến người dùng
        public Project? Project { get; set; } // Liên kết đến dự án
    }
}
