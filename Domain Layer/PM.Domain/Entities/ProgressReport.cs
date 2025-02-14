using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM.Domain.Entities
{
    public class ProgressReport
    {
        [Key]
        public string Id { get; set; } // Mã báo cáo
        [ForeignKey(nameof(Plan))]
        public string PlanId { get; set; } // Mã kế hoạch
        public string ReportDetails { get; set; } // Chi tiết báo cáo
        public DateTime ReportDate { get; set; } // Ngày báo cáo

        public Plan Plan { get; set; } // Liên kết đến kế hoạch
    }
}
