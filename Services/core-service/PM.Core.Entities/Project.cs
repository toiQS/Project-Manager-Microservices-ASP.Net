using PM.Shared.Dtos.cores;
using System.ComponentModel.DataAnnotations;

namespace PM.Core.Entities
{
    public class Project
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public TypeStatus Status { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsModified { get; set; }
        public bool IsLocked { get; set; }
        public bool IsComplied { get; set; }
    }
}
