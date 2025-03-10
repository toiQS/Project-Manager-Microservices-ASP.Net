using PM.Domain.Entities;
using PM.Domain.Models.members;
using PM.Domain.Models.missions;
using PM.Domain.Models.progressReports;

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
        public ICollection<IndexMission> Missions { get; set; } = new List<IndexMission>(); // Các nhiệm vụ
        public ICollection<IndexRepost> ProgressReports { get; set; } = new List<IndexRepost>(); // Báo cáo tiến độ
    }
}
