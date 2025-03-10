using PM.Domain.Models.members;
using PM.Domain.Models.plans;

namespace PM.Domain.Models.projects
{
    public class DetailProject
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsDeleted { get; set; } // Đã xóa chưa
        public bool IsCompleted { get; set; } // Đã hoàn thành chưa
        public string OwnerName { get; set; } = string.Empty; //user name 
        public string OwnerAvata { get; set; } = string.Empty;
        public List<IndexPlan> Plans { get; set; } = new List<IndexPlan>();
        public List<IndexMember> Members { get; set; } = new List<IndexMember> { };
        public string Status { get; set; } = string.Empty;
        public int QuantityMember { get; set; } = 0;
        public string ProjectDescription { get; set; } = string.Empty;
    }
}
