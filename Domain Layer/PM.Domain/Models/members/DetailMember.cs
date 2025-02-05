using PM.Domain.Models.tasks;

namespace Shared.member
{
    public class DetailMember
    {
        public string RoleUserInProjectId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserAvata {  get; set; } = string.Empty ;
        public string RoleUserNameInProject { get; set; } = string.Empty ;
        public List<IndexTask> Tasks { get; set; } = new List<IndexTask>();
    }
}
