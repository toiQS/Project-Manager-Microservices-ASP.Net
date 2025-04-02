using System.ComponentModel.DataAnnotations;

namespace Shared.Core.Entities.Status
{
    public class Status
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;
    }
}
