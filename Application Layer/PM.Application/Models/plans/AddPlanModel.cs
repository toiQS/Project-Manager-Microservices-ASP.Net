namespace PM.Application.Models.plans
{
    public class AddPlanModel
    {
        public string MemberId { get; set; } = string.Empty;
        public string ProjectId { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly StartAt { get; set; }
        public DateOnly EndAt { get; set; }
    }
}
