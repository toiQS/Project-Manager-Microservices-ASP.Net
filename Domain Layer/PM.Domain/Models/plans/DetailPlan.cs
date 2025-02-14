using PM.Domain.Entities;

namespace PM.Domain.Models.plans
{
    public class DetailPlan
    {
        public string PlanId { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public string Description {  get; set; } = string.Empty;
        public DateOnly StartDate { get; set; } // Ngày bắt đầu
        public DateOnly EndDate { get; set; } // Ngày kết thúc
        public ICollection<Mission> Missions { get; set; } // Các nhiệm vụ
        public ICollection<ProgressReport> ProgressReports { get; set; } // Báo cáo tiến độ
    }
}
