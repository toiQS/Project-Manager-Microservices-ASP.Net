using PM.Domain.Models.members;

namespace PM.Domain.Models.missions
{
    public class UpdateMission
    {
        public string TaskName { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty ;
        public DateOnly StartAt { get; set; }
        public DateOnly EndAt { get; set; }
        //public bool IsDone { get; set; }    
        public List<IndexMember> Members { get; set; }  = new List<IndexMember>();
    }
}
