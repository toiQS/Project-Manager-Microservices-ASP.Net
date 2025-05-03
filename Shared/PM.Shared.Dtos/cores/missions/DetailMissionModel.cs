namespace PM.Shared.Dtos.cores.missions
{
    public class DetailMissionModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Goal { get; set; } = string.Empty;
        public string CreateBy { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
