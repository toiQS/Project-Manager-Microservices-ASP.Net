namespace PM.Domain.Models.plans
{
    public class AddPlan
    {
        public string PlanName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly StartAt { get; set; }
        public DateOnly EndAt { get; set; } 
    }
}
