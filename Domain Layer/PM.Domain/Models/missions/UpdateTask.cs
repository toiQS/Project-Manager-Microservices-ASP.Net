using PM.Domain.Models.members;

namespace PM.Domain.Models.missions
{
    public class UpdateTask
    {
        public string TaskName { get; set; } = string.Empty;
        public string TaskDescription { get; set; } = string.Empty ;
        //public bool IsDone { get; set; }    
        public List<IndexMember> Members { get; set; }  = new List<IndexMember>();
    }
}
