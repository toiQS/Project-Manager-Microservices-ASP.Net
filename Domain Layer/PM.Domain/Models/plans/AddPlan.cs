namespace PM.Domain.Models.plans
{
    public class AddPlan
    {
        public string PlanName { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
    }
}
