namespace PM.Shared.Dtos.cores.missions
{
    public class PatchMissionModel
    {
        public string Name { get; set; } = string.Empty;
        public string Request {  get; set; } = string.Empty;
        //public string PlanId { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
