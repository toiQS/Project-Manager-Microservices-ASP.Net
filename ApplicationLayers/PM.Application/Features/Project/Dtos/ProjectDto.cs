using System;

namespace PM.Application.Features.Project.Dtos
{
    public class ProjectDto
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int StatusId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsCompleted { get; set; }
    }
}
