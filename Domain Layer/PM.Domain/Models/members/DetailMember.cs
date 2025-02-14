using PM.Domain.Models.missions;

namespace Shared.member
{
    public class DetailMember
    {
        public string MemberId {  get; set; }
        public string MemberName { get; set; }
        public string RoleMember {  get; set; }
        public string PositionWork { get; set; }
        public List<IndexMission> missions { get; set; }
    }
}
