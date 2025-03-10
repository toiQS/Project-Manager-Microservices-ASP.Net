using PM.Domain.Models.missions;

namespace Shared.member
{
    public class DetailMember
    {
        public string MemberId {  get; set; } = string.Empty;
        public string MemberName { get; set; } = string.Empty;
        public string RoleMember {  get; set; } = string.Empty;
        public string PositionWork { get; set; } = string.Empty;
        public List<IndexMission> Missions { get; set; } = new List<IndexMission>();
    }
}
