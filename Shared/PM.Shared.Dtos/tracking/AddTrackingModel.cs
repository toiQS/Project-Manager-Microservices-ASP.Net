using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Shared.Dtos.tracking
{
    public class AddTrackingModel
    {
        public string UserId { get; set; } = string.Empty;
        public string ProjectId { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
    }
}
