using System.ComponentModel.DataAnnotations;

namespace PM.Core.Entities
{
    public class Mission
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Request {  get; set; } = string.Empty;
        public string PlanId {  get; set; } = string.Empty;
        public string ProjectMemberId {  get; set; } = string.Empty;
        public TypeStatus Status { get; set; }
        //public string CreateBy { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Plan Plan { get; set; }
        public ProjectMember ProjectMember { get; set; }
    }
}
