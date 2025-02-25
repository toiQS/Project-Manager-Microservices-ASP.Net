namespace PM.Application.Models.missions
{
    public class AddMissonModel
    {
        public string MemberId { get; set; } = string.Empty;
        public string PlanId { get; set; } = string.Empty;
        public string TaskName { get; set; } = string.Empty;
        public DateOnly StartAt { get; set; }
        public DateOnly EndAt { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
