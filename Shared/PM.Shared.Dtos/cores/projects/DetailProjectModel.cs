using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Shared.Dtos.cores.projects
{
    public class DetailProjectModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public bool IsModified { get; set; }
        public bool IsLocked { get; set; }
        public bool IsComplied { get; set; }
    }
}
