using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM.Document.Domain.Entities
{
    public class Document
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Path { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        public string ProjectId { get; set; } = string.Empty.ToString();

        public string MissionId { get; set; } = string.Empty.ToString();

    }
}
