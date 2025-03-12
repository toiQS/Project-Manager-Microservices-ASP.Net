using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM.Domain.Entities
{
    public class Plan
    {
        [Key]
        public string Id { get; set; } // Mã kế hoạch
        //[ForeignKey(nameof(Project))]
        public string ProjectId { get; set; } // Mã dự án
        public string Name { get; set; } // Tên kế hoạch
        public string Description { get; set; } // miêu tả kể hoạch
        public DateTime StartDate { get; set; } // Ngày bắt đầu
        public DateTime EndDate { get; set; } // Ngày kết thúc
        //[ForeignKey(nameof(Status))]
        public int StatusId { get; set; } // ID tình trạng
        public bool IsCompleted { get; set; } // Đã hoàn thành chưa

        public Project Project { get; set; } // Liên kết đến dự án
        public Status Status { get; set; } // Liên kết đến tình trạng
        public ICollection<Mission> Missions { get; set; } // Các nhiệm vụ
        public ICollection<ProgressReport> ProgressReports { get; set; } // Báo cáo tiến độ
    }
}

