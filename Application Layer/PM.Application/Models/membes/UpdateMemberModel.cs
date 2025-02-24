using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Application.Models.membes
{
public    class UpdateMemberModel
    {
        public string MemberCurrentId { get; set; } = string.Empty;
        public string MemberId { get; set; } = string.Empty;
        public string PositionWork { get; set; } = string.Empty;
        public string ProjectId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
