using Microsoft.AspNetCore.Mvc;
using PM.Application.Models.membes;

namespace PM.Application.Interfaces
{
    public interface IMemberLogic
    {
        public Task<IActionResult> GetMembers();
        public Task<IActionResult> GetMembers(string projectId);
        public Task<IActionResult> GetMember(string memberId);
        public Task<IActionResult> AddMember(AddMemberModel model);
        public Task<IActionResult> DeleteMember(DeleteMemberModel model);
        public Task<IActionResult> UpdateMember(UpdateMemberModel model);
    }
}
