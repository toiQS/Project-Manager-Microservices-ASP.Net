using Shared.Core.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Core.Entities.Projects
{
    public class Project : AuditableEntity
    {
        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        public bool IsCompleted { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Guid StatusId { get; set; }
    }

}