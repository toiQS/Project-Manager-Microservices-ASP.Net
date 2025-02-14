using PM.Domain.Models.members;

namespace PM.Domain.Models.missions
{
    public class AddTask
    {
        public string TaskName { get; set; } = string.Empty;
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<IndexMember> IndexMembers { get; set; } = new List<IndexMember>();
    }
}
