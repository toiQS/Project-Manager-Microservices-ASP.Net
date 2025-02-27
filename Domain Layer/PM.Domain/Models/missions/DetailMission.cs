using PM.Domain.Models.members;
using PM.Domain.Models.missions.members;

namespace PM.Domain.Models.missions
{
    public class DetailMission
    {
        public string MissionId { get; set; } = string.Empty;
        public string MissionName { get; set; } = string.Empty;
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public DateTime CreateAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsDone { get; set; }
        public List<IndexMemberMission> IndexMembers { get; set; } = new List<IndexMemberMission>();
    }
}
