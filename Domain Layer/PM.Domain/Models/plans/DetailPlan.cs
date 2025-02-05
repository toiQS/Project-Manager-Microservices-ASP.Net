using PM.Domain.Models.tasks;

namespace PM.Domain.Models.plans
{
    public class DetailPlan
    {
        public string PlanId { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsDone { get; set; }
        public List<IndexTask> Tasks { get; set; } = new List<IndexTask>();
    }
}
