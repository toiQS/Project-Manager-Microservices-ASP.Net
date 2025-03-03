namespace PM.Models.missions
{
    public class AddMembertMissionModel
    {
        public string MemeberId { get; set; } = string.Empty;
        public string MissisonId { get; set; } = string.Empty;
        public List<string> MemberIds { get; set; } = new List<string>();

    }
}
