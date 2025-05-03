using System;
using System.ComponentModel.DataAnnotations;

namespace PM.Application.Features.Project.Dtos
{
    public class ProjectCreateDto
    {
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
    }
}
