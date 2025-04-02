using Shared.Core.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Core.Entities.Projects
{
    public class ProjectMember : BaseEntity
    {
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }

        [MaxLength(255)]
        public string PositionWork { get; set; } = string.Empty;
    }

}
