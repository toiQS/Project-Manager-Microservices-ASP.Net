using System.ComponentModel.DataAnnotations;

namespace PM.Core.Entities
{
    public class Position
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
