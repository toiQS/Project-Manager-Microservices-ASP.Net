using Azure.Core;
using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Models.members;
using PM.Domain.Models.projects;
using System.ComponentModel.DataAnnotations;
using System.Formats.Asn1;

namespace PM.Persistence.Implements.Services
{
    public class ProjectServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private string _ownRoleId;
        #region get list project user has joined
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProductListUserHasJoined(string userId)
        {
            if (userId == null) return ServicesResult<IEnumerable<IndexProject>>.Failure("");
            var response = new List<IndexProject>();
            try
            {
                // khởi động việc lấy thông tin vai trò
                if ((await GetOwnRole()).Status == false) return ServicesResult<IEnumerable<IndexProject>>.Failure((await GetOwnRole()).Message);
                //kiểm tra người dùng
                var userCheck = await _unitOfWork.UserRepository.ExistsAsync(userId);
                if (userCheck == false) return ServicesResult<IEnumerable<IndexProject>>.Failure("No find user in database");
                //lấy danh sách dự án người dùng tham gia
                var projects = await _unitOfWork.ProjectRepository.GetManyByKeyAndValue("UserId", userId);
                if (projects.Status == false) return ServicesResult<IEnumerable<IndexProject>>.Failure(projects.Message);
                if (projects.Data == null) return ServicesResult<IEnumerable<IndexProject>>.Success(response);
                foreach (var itemProject in projects.Data)
                {
                    var member = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId", itemProject.Id);
                    if (member.Status == false) return ServicesResult<IEnumerable<IndexProject>>.Failure("Can't get any member in project");
                    var ownerProjectId = member.Data.Where(x => x.RoleId == _ownRoleId).FirstOrDefault().Id ?? string.Empty;
                    if (ownerProjectId == null) return ServicesResult<IEnumerable<IndexProject>>.Failure("No find owner this project");
                    var user = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);
                    if (user.Status == false) return ServicesResult<IEnumerable<IndexProject>>.Failure(user.Message);
                    var indexProject = new IndexProject()
                    {
                        ProjectId = itemProject.Id,
                        ProjectName = itemProject.Name,
                        OwnerAvata = user.Data.AvatarPath,
                        OwnerName = user.Data.NickName,
                    };
                    response.Add(indexProject);
                }
                return ServicesResult<IEnumerable<IndexProject>>.Success(response);

            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure("");
            }
            finally
            {
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Dispose();
            }
        }
        #endregion
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProductListUserHasOwner(string userId)
        {
            if (userId == null) return ServicesResult<IEnumerable<IndexProject>>.Failure("");
            var response = new List<IndexProject>();
            try
            {
                if ((await GetOwnRole()).Status == false) return ServicesResult<IEnumerable<IndexProject>>.Failure((await GetOwnRole()).Message);
                var projects = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("UserId", userId);
                if (projects.Status == false) return ServicesResult<IEnumerable<IndexProject>>.Failure(projects.Message);
                var ownerProject = projects.Data.Where(x => x.RoleId == _ownRoleId).ToList();
                if (ownerProject == null) return ServicesResult<IEnumerable<IndexProject>>.Success(response);
                var user = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);
                if (user.Status == false) return ServicesResult<IEnumerable<IndexProject>>.Failure(user.Message);
                foreach (var item in ownerProject)
                {
                    var project = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", item.ProjectId);
                    if (project.Status == false) return ServicesResult<IEnumerable<IndexProject>>.Failure(project.Message);
                    var indexProject = new IndexProject()
                    {
                        ProjectId = item.ProjectId,
                        OwnerName = user.Data.NickName,
                        OwnerAvata = user.Data.AvatarPath,
                        ProjectName = project.Data.Name
                    };
                    response.Add(indexProject);
                }
                return ServicesResult<IEnumerable<IndexProject>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexProject>>.Failure("");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        public async Task<ServicesResult<DetailProject>> GetDetailProject(string userId, string projectId)
        {
            if (userId == null || projectId == null) return ServicesResult<DetailProject>.Failure("");
            try
            {

            }
            catch (Exception ex)
            {
                return ServicesResult<DetailProject>.Failure(ex.Message);
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        public async Task<ServicesResult<DetailProject>> Add(string userId, AddProject addProject)
        {
            if (string.IsNullOrEmpty(userId) || addProject == null) return ServicesResult<DetailProject>.Failure("");
            try
            {
                if ((await GetOwnRole()).Status == false) return ServicesResult<DetailProject>.Failure((await GetOwnRole()).Message);
                var projectUser = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("UserId", userId);
                if (projectUser.Status == false) return ServicesResult<DetailProject>.Failure(projectUser.Message);
                var projects = projectUser.Data.Where(x => x.RoleId == _ownRoleId).ToList();
                if (!projects.Any())
                    return await AddMethodSupport(userId, addProject);
                foreach (var item in projects)
                {
                    var project = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", item.ProjectId);
                    if (project.Status == false) return ServicesResult<DetailProject>.Failure(project.Message);
                    if (project.Data.Name == addProject.ProjectName) return ServicesResult<DetailProject>.Failure("Project Name is existed");
                }
                return await AddMethodSupport(userId, addProject);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailProject>.Failure(ex.Message);
            }
            finally
            {
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Dispose();
            }
        }
        public async Task<ServicesResult<DetailProject>> UpdateInfo(string userId, string projectId, UpdateProject updateProject)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId) || updateProject == null) return ServicesResult<DetailProject>.Failure("");
            try
            {
                if ((await GetOwnRole()).Status == false) return ServicesResult<DetailProject>.Failure((await GetOwnRole()).Message);
                // lấy danh sách dự án người dùng có tham gia
                var projectUser = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("UserId", userId);
                if (projectUser.Status == false) return ServicesResult<DetailProject>.Failure(projectUser.Message);

                if(!projectUser.Data.Any()) return await UpdateMethodSupport(userId, projectId, updateProject);
                
                // kiểm tra vai trò theo của người dùng trong dự án
                var checkProjectIsExitsted = projectUser.Data.Any(x => x.RoleId == _ownRoleId && x.ProjectId == projectId);
                if (checkProjectIsExitsted == false) return ServicesResult<DetailProject>.Failure("");
                // kiểm tra tên dự án có tồn tại chưa
                foreach (var item in projectUser.Data)
                {
                    var project = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", item.ProjectId);
                    if (project.Status == false) return ServicesResult<DetailProject>.Failure(project.Message);
                    if (project.Data.Name == updateProject.ProjectName) return ServicesResult<DetailProject>.Failure("");
                }
                return await UpdateMethodSupport(userId, projectId, updateProject);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailProject>.Failure(ex.Message);
            }
            finally
            {
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Dispose();
            }
        }
        //public Task<ServicesResult<IEnumerable<IndexProject>> Delete(string userId, string projectId);
        public Task<ServicesResult<DetailProject>> UpdateIsDelete(string userId, string projectId);
        public Task<ServicesResult<DetailProject>> UpdateIsAccessed(string userId, string projectId);
        public Task<ServicesResult<DetailProject>> UpdateIsDone(string userId, string projectId);
        public Task<ServicesResult<DetailProject>> UpdateStatus(string userId, string projectId);
        private async Task<ServicesResult<bool>> GetOwnRole()
        {
            try
            {
                var ownRole = await _unitOfWork.RoleInProjectRepository.GetOneByKeyAndValue("Name", "Owner");
                if (ownRole.Status == false) return ServicesResult<bool>.Failure(ownRole.Message);
                _ownRoleId = ownRole.Data.Id;
                return ServicesResult<bool>.Success(ownRole.Status);
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Failure(ex.Message);
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        private async Task<ServicesResult<DetailProject>> AddMethodSupport(string userId, AddProject addProject)
        {
            try
            {
                // add new project
                var project = new Project()
                {
                    Id = $"",
                    Name = addProject.ProjectName,
                    StartDate = addProject.StartAt,
                    EndDate = addProject.EndAt,
                    CreatedDate = DateTime.Now,
                    IsCompleted = false,
                    IsDeleted = false,
                    StatusId = DateTime.Now == addProject.StartAt
                    ? 3 // Ongoing
                    : (DateTime.Now < addProject.StartAt ? 2 : 1) // Upcoming or Overdue
                };
                var responseProject = await _unitOfWork.ProjectRepository.AddAsync(project);
                if (!responseProject.Status) return ServicesResult<DetailProject>.Failure(responseProject.Message);


                // set up owner role 
                var member = new ProjectMember()
                {
                    Id = "",
                    ProjectId = project.Id,
                    RoleId = _ownRoleId,
                    UserId = userId,
                    PositionWork = string.Empty
                };
                var projectMemberResponse = await _unitOfWork.ProjectMemberRepository.AddAsync(member);
                if (!projectMemberResponse.Status) return ServicesResult<DetailProject>.Failure(projectMemberResponse.Message);

                var user = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);
                if (!user.Status) return ServicesResult<DetailProject>.Failure(user.Message);

                // create a activity log for project and owner role project
                var activityProject = new ActivityLog()
                {
                    Id = "",
                    ActionDate = DateTime.Now,
                    Action = $"A new project was created by {user.Data.NickName}",
                    ProjectId = project.Id,
                };
                var acvitity = await _unitOfWork.ActivityLogRepository.AddAsync(activityProject);
                if (!acvitity.Status) return ServicesResult<DetailProject>.Failure(acvitity.Message);

                // xác thực thành công

                var indexMember = new IndexMember()
                {
                    PositionWorkName = member.PositionWork,
                    UserName = user.Data.NickName,
                    RoleUserInProjectId = member.Id,
                    UserAvata = user.Data.AvatarPath
                };

                // tạo kết quả trả về
                var response = new DetailProject()
                {
                    ProjectId = project.Id,
                    OwnerName = user.Data.NickName,
                    OwnerAvata = user.Data.AvatarPath,
                    CreateAt = project.CreatedDate,
                    EndAt = project.EndDate,
                    ProjectName = project.Name,
                    IsCompleted = project.IsCompleted,
                    IsDeleted = project.IsDeleted,
                };
                response.Members.Add(indexMember);

                return ServicesResult<DetailProject>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailProject>.Failure(ex.Message);
            }
            finally
            {
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Dispose();
            }
        }
        private async Task<ServicesResult<DetailProject>> UpdateMethodSupport(string userId, string projectId, UpdateProject updateProject)
        {
            try
            {   
                var project = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id",projectId);
                if (project.Status == false) return ServicesResult<DetailProject>.Failure(project.Message);
                project.Data.Name = updateProject.ProjectName ?? project.Data.Name;
                project.Data.Description = updateProject.ProjectDescription??project.Data.Description;
                project.Data.StartDate = updateProject.StartDate?? project.Data.StartDate;
                project.Data.EndDate = updateProject.EndDate?? project.Data.EndDate;
                project.Data.StatusId = DateTime.Now == updateProject.StartDate
                    ? 3 // Ongoing
                    : (DateTime.Now < updateProject.StartDate ? 2 : 1);// Upcoming or Overdue

                var update = await _unitOfWork.ProjectRepository.UpdateAsync(project.Data);
                if(!update.Status) return ServicesResult<DetailProject>.Failure($"{update.Message}");
                var user = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", userId);

                if (!user.Status) return ServicesResult<DetailProject>.Failure(user.Message);
                // create a activity log for project and owner role project
                var activityProject = new ActivityLog()
                {
                    Id = "",
                    ActionDate = DateTime.Now,
                    Action = $"A new project was created by {user.Data.NickName}",
                    ProjectId = project.Data.Id,
                };
                var acvitity = await _unitOfWork.ActivityLogRepository.AddAsync(activityProject);
                if (!acvitity.Status) return ServicesResult<DetailProject>.Failure(acvitity.Message);

                // xác thực thành công

                
                // tạo kết quả trả về
                var response = new DetailProject()
                {
                    ProjectId = project.Data.Id,
                    OwnerName = user.Data.NickName,
                    OwnerAvata = user.Data.AvatarPath,
                    CreateAt = project.Data.CreatedDate,
                    EndAt = project.Data.EndDate,
                    ProjectName = project.Data.Name,
                    IsCompleted = project.Data.IsCompleted,
                    IsDeleted = project.Data.IsDeleted,
                };
                var memberProject = await _unitOfWork.ProjectMemberRepository.GetManyByKeyAndValue("ProjectId", projectId);
                if (!memberProject.Status) return ServicesResult<DetailProject>.Failure(memberProject.Message);
                foreach (var member in memberProject.Data)
                {
                    var infoMember = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", member.UserId);
                    if (!infoMember.Status) return ServicesResult<DetailProject>.Failure(infoMember.Message);
                    var index = new IndexMember()
                    {
                        PositionWorkName = member.PositionWork,
                        UserName = infoMember.Data.NickName,
                        RoleUserInProjectId = member.Id,
                        UserAvata = infoMember.Data.AvatarPath,
                    };
                    response.Members.Add(index);
                }
                return ServicesResult<DetailProject>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<DetailProject>.Failure(ex.Message);
            }
            finally
            {
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Dispose();
            }
        }
    }
}
