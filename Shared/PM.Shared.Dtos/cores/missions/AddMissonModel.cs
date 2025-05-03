namespace PM.Shared.Dtos.cores.missions
{
    public class AddMissonModel
    {
        public string Name { get; set; } = string.Empty;
        public string Request { get; set; } = string.Empty;
        public string PlanId { get; set; } = string.Empty;
        public string ProjectMemberId { get; set; } = string.Empty;
        public TypeStatus Status { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
