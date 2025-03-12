using System.ComponentModel.DataAnnotations;

namespace PM.Domain.Entities
{
    public class Status
    {
        [Key]
        public int Id { get; set; } // Mã tình trạng
        public string Name { get; set; } // Tên tình trạng
    }
}
