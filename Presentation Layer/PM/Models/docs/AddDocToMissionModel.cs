namespace PM.Models.docs
{
    public class AddDocToMissionModel
    {
        public string MemberId { get; set; } = string.Empty;
        public string MissionId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
    }
}
