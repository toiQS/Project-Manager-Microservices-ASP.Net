﻿using PM.Domain.Models.members;
using Shared.member;

namespace PM.Domain.Interfaces.Services
{
    public interface IMemberServices
    {
        public Task<ServicesResult<IEnumerable<IndexMember>>> GetMembers();
        public Task<ServicesResult<IEnumerable<IndexMember>>> GetMemberInProject(string projectId);
        public Task<ServicesResult<DetailMember>> GetDetailMember(string memberId);
        public Task<ServicesResult<DetailMember>> AddMember(string memberCurrentId, string projectId, AddMember addMember);
        public Task<ServicesResult<DetailMember>> UpdateMember(string memberCurrentId, string memberId, UpdateMember updateMember);
        public Task<ServicesResult<IEnumerable<IndexMember>>> DeleteMember(string memberCurrentId, string memberId);
        public Task<ServicesResult<bool>> DeleteMemberFunc(string memberCurrentId, string memberId);
    }
}
