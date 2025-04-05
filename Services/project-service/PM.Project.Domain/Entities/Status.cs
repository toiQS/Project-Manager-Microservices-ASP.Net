using System;
using System.ComponentModel.DataAnnotations;

namespace PM.Project.Domain.Entities
{
    public class Status
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;
    }
}
