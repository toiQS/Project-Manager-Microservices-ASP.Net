using PM.Domain.Models.members;

namespace PM.Domain.Models.missions
{
    public class AddMission
    {
        public string TaskName { get; set; } = string.Empty;
        public DateOnly StartAt { get; set; }
        public DateOnly EndAt { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<IndexMember> IndexMembers { get; set; } = new List<IndexMember>();
    }
}
