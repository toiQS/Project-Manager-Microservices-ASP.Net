namespace PM.Domain.Models.plans
{
    public class AddPlan
    {
        public string PlanName { get; set; }
        public string Description { get; set; }
        public DateOnly StartAt { get; set; }
        public DateOnly EndAt { get; set; }
    }
}
