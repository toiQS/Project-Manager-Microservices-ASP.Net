using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PM.Domain.Entities
{
    public class Document
    {
        [Key]
        public string Id { get; set; } // Mã tài liệu
        public string Name { get; set; } // Tên tài liệu
        public string Path { get; set; } // Đường dẫn tài liệu
        public string Descriotion { get; set; } // Miêu tả tài liệu 
        [ForeignKey(nameof(Project))]
        public string ProjectId { get; set; } = string.Empty; // Liên kết đến dự án (nếu có)
        [ForeignKey(nameof(Mission))]
        public string MissionId { get; set; } = string.Empty; // Liên kết đến nhiệm vụ (nếu có)

        public Project? Project { get; set; } // Liên kết đến dự án
        public Mission ?Mission { get; set; } // Liên kết đến nhiệm vụ
    }
}
