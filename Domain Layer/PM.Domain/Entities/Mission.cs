using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace PM.Domain.Entities
{
    public class Mission
    {
        [Key]
        public string Id { get; set; } = string.Empty;// Mã nhiệm vụ
        [ForeignKey(nameof(Plan))]
        public string PlanId { get; set; } = string.Empty;// Mã kế hoạch
        public string Name { get; set; } = string.Empty;// Tên nhiệm vụ
        public string Description { get; set; } = string.Empty;// Mô tả nhiệm vụ
        public DateTime StartDate { get; set; } // Ngày bắt đầu
        public DateTime EndDate { get; set; } // Ngày kết thúc
        public DateTime CreateDate { get; set; } // Ngày tạo
        [ForeignKey(nameof(Status))]
        public int StatusId { get; set; } // ID tình trạng
        public bool IsCompleted { get; set; } // Đã hoàn thành chưa

        public Plan Plan { get; set; }// Liên kết đến kế hoạch
        public Status Status { get; set; } // Liên kết đến tình trạng
        public ICollection<MissionAssignment> Assignments { get; set; } // Giao nhiệm vụ
        public ICollection<Document> Documents { get; set; } // Tài liệu nhiệm vụ
    }
}
