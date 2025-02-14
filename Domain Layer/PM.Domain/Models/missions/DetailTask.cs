using PM.Domain.Models.members;

namespace PM.Domain.Models.missions
{
    public class DetailTask
    {
        public string TaskId { get; set; } = string.Empty;
        public string TaskName { get; set; } = string.Empty;
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public DateTime CreateAt { get; set; }
        public string Status { get; set; } = string.Empty;

        public bool IsDone { get; set; }
        public List<IndexMember> IndexMembers { get; set; } = new List<IndexMember>();
    }
}
