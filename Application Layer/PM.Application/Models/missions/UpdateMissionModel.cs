using PM.Domain.Models.members;

namespace PM.Application.Models.missions
{
    public class UpdateMissionModel
    {
        public string MemnberId { get; set; } = string.Empty;
        public string MissionId { get; set; } = string.Empty;
        public string TaskName { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty;
        public DateOnly StartAt { get; set; }
        public DateOnly EndAt { get; set; }
        //public bool IsDone { get; set; }    
        //public List<IndexMember> Members { get; set; } = new List<IndexMember>();
    }
}
