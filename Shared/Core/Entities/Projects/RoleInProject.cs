using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.Core.Entities.Projects
{
    public class RoleInProject
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
    }
}
