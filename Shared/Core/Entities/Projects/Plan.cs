using Shared.Core.Entities.Status;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Core.Entities.Projects
{
    public class Plan
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); // Sử dụng Guid thay vì string

        [Required]
        public Guid ProjectId { get; set; } // Chuyển về Guid

        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        public DateTime? StartDate { get; set; } // Nullable để tránh lỗi mặc định

        public DateTime? EndDate { get; set; } // Nullable để tránh lỗi mặc định

        public int StatusId { get; set; }

        public bool IsCompleted { get; set; } = false;

        // Ràng buộc Foreign Key chặt chẽ hơn
        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; } = null!;

        
    }
}
