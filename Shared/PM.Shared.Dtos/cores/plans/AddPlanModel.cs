namespace PM.Shared.Dtos.cores.plans
{
    public class AddPlanModel
    {
        public string Name { get; set; } = string.Empty;
        public string Goal { get; set; } = string.Empty;
        public string ProjectId { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
