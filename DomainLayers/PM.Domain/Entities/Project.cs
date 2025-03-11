using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM.Domain.Entities
{
    public class Project
    {
        [Key]
        public string Id { get; set; } // Mã dự án
        public string Name { get; set; } // Tên dự án
        public string Description { get; set; } // Miêu tả dự án
        [ForeignKey(nameof(Status))]
        public int StatusId { get; set; } // ID tình trạng
        public DateTime CreatedDate { get; set; } // Ngày tạo
        public DateTime? StartDate { get; set; } // Ngày bắt đầu
        public DateTime? EndDate { get; set; } // Ngày kết thúc
        public bool IsDeleted { get; set; } // Đã xóa chưa
        public bool IsCompleted { get; set; } // Đã hoàn thành chưa

        public Status Status { get; set; } // Liên kết đến tình trạng
        public ICollection<ProjectMember> Members { get; set; } // Thành viên dự án
        public ICollection<Plan> Plans { get; set; } // Các kế hoạch
        public ICollection<Document> Documents { get; set; } // Tài liệu dự án
        public ICollection<ActivityLog> ActivityLogs { get; set; } // Nhật ký hoạt động
    }
}
