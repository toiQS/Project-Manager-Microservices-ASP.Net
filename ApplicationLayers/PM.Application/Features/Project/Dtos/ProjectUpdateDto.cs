using System;
using System.ComponentModel.DataAnnotations;

namespace PM.Application.Features.Project.Dtos
{
    public class ProjectUpdateDto
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int StatusId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        [Required]
        public bool IsCompleted { get; set; }
    }
}
