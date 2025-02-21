using PM.Domain;
using PM.Domain.Entities;
using PM.Domain.Interfaces;
using PM.Domain.Interfaces.Services;
using PM.Domain.Models.reports;

namespace PM.Persistence.Implements.Services
{
    public class ReportServices : IReportServices
    {
        private readonly IUnitOfWork _unitOfWork;
        public ReportServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        private string _ownerId, _leaderId, _managerId, _memberId;
        #region Retrieves all progress reports associated with a given plan.
        /// <summary>
        /// Retrieves all progress reports associated with a given plan.
        /// </summary>
        /// <param name="planId">The ID of the plan for which reports are retrieved.</param>
        /// <returns>A service result containing a list of reports or an error message.</returns>
        public async Task<ServicesResult<IEnumerable<IndexReport>>> GetReportsInPlan(string planId)
        {
            // Validate input
            if (string.IsNullOrEmpty(planId))
                return ServicesResult<IEnumerable<IndexReport>>.Failure("Plan ID cannot be null or empty.");

            try
            {
                // Fetch reports associated with the given plan ID
                var reportsResult = await _unitOfWork.ProgressReportRepository.GetManyByKeyAndValue("PlanId", planId);

                // Check if retrieval was successful
                if (!reportsResult.Status)
                    return ServicesResult<IEnumerable<IndexReport>>.Failure($"Failed to retrieve reports: {reportsResult.Message}");

                // Transform retrieved reports into response format
                var response = reportsResult.Data.Select(report => new IndexReport
                {
                    Id = report.Id,
                    DateTime = report.ReportDate,
                    ReportDetail = report.ReportDetails,
                });

                return ServicesResult<IEnumerable<IndexReport>>.Success(response);
            }
            catch (Exception ex)
            {
                return ServicesResult<IEnumerable<IndexReport>>.Failure($"An unexpected error occurred: {ex.Message}");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        #endregion

        #region Adds a new progress report for a given plan and logs the action.
        /// <summary>
        /// Adds a new progress report for a given plan and logs the action.
        /// </summary>
        /// <param name="memberId">The ID of the member adding the report.</param>
        /// <param name="planId">The ID of the plan for which the report is created.</param>
        /// <param name="reportDetail">The details of the report.</param>
        /// <returns>A service result indicating success or failure with a message.</returns>
        public async Task<ServicesResult<string>> AddReport(string memberId, string planId, string reportDetail)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(memberId) || string.IsNullOrEmpty(planId) || reportDetail == null)
                return ServicesResult<string>.Failure("Invalid input parameters.");

            try
            {
                // Validate member role
                var memberRoleResult = await GetMemberRole();
                if (!memberRoleResult.Status)
                    return ServicesResult<string>.Failure($"Failed to validate member role: {memberRoleResult.Message}");

                // Retrieve plan details
                var planResult = await _unitOfWork.PlanRepository.GetOneByKeyAndValue("Id", planId);
                if (!planResult.Status)
                    return ServicesResult<string>.Failure($"Failed to retrieve plan: {planResult.Message}");

                // Retrieve project details
                var projectResult = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", planResult.Data.ProjectId);
                if (!projectResult.Status)
                    return ServicesResult<string>.Failure($"Failed to retrieve project: {projectResult.Message}");

                // Retrieve member details
                var memberResult = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberId);
                if (!memberResult.Status)
                    return ServicesResult<string>.Failure($"Failed to retrieve member: {memberResult.Message}");

                // Check if the member is part of the project and has the required role
                if (memberResult.Data.ProjectId != projectResult.Data.Id || memberResult.Data.RoleId != _memberId)
                    return ServicesResult<string>.Failure("Member does not have sufficient permissions to add a report.");

                // Create a new progress report
                var report = new ProgressReport
                {
                    Id = Guid.NewGuid().ToString(),
                    PlanId = planId,
                    ReportDetails = reportDetail,
                    ReportDate = DateTime.Now,
                };

                var addReportResult = await _unitOfWork.ProgressReportRepository.AddAsync(report);
                if (!addReportResult.Status)
                    return ServicesResult<string>.Failure($"Failed to add report: {addReportResult.Message}");

                // Retrieve user info for logging
                var userResult = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", memberResult.Data.UserId);
                if (!userResult.Status)
                    return ServicesResult<string>.Failure($"Failed to retrieve user information: {userResult.Message}");

                // Create a log entry for the report creation
                var log = new ActivityLog
                {
                    Id = Guid.NewGuid().ToString(),
                    ProjectId = projectResult.Data.Id,
                    UserId = memberResult.Data.UserId,
                    ActionDate = DateTime.Now,
                    Action = $"A new report was created by {userResult.Data.UserName} in plan {planResult.Data.Name}."
                };

                var addLogResult = await _unitOfWork.ActivityLogRepository.AddAsync(log);
                if (!addLogResult.Status)
                    return ServicesResult<string>.Failure($"Failed to log activity: {addLogResult.Message}");

                return ServicesResult<string>.Success("Report added successfully.");
            }
            catch (Exception ex)
            {
                return ServicesResult<string>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
        #endregion
        #region Updates the details of a progress report and logs the action.
        /// <summary>
        /// Updates the details of a progress report and logs the action.
        /// </summary>
        /// <param name="memberId">The ID of the member performing the update.</param>
        /// <param name="reportId">The ID of the report to update.</param>
        /// <param name="reportDetail">The updated report details.</param>
        /// <returns>A service result indicating success or failure with a message.</returns>
        public async Task<ServicesResult<string>> UpdateReport(string memberId, string reportId, string reportDetail)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(memberId) || string.IsNullOrEmpty(reportId) || string.IsNullOrEmpty(reportDetail))
                return ServicesResult<string>.Failure("Invalid input parameters.");

            try
            {
                // Validate member role
                var memberRoleResult = await GetMemberRole();
                if (!memberRoleResult.Status)
                    return ServicesResult<string>.Failure($"Failed to validate member role: {memberRoleResult.Message}");

                // Retrieve the report to update
                var reportResult = await _unitOfWork.ProgressReportRepository.GetOneByKeyAndValue("Id", reportId);
                if (!reportResult.Status)
                    return ServicesResult<string>.Failure($"Failed to retrieve report: {reportResult.Message}");

                // Retrieve the plan associated with the report
                var planResult = await _unitOfWork.PlanRepository.GetOneByKeyAndValue("Id", reportResult.Data.PlanId);
                if (!planResult.Status)
                    return ServicesResult<string>.Failure($"Failed to retrieve plan: {planResult.Message}");

                // Retrieve the project associated with the plan
                var projectResult = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", planResult.Data.ProjectId);
                if (!projectResult.Status)
                    return ServicesResult<string>.Failure($"Failed to retrieve project: {projectResult.Message}");

                // Retrieve the member performing the update
                var memberResult = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberId);
                if (!memberResult.Status)
                    return ServicesResult<string>.Failure($"Failed to retrieve member: {memberResult.Message}");

                // Verify that the member belongs to the project and has the required role
                if (memberResult.Data.ProjectId != projectResult.Data.Id || memberResult.Data.RoleId != _memberId)
                    return ServicesResult<string>.Failure("Member does not have sufficient permissions to update this report.");

                // Update report details
                reportResult.Data.ReportDetails = reportDetail;

                // Save the updated report
                var updateResponse = await _unitOfWork.ProgressReportRepository.UpdateAsync(reportResult.Data);
                if (!updateResponse.Status)
                    return ServicesResult<string>.Failure($"Failed to update report: {updateResponse.Message}");

                // Retrieve user info for logging
                var userResult = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", memberResult.Data.UserId);
                if (!userResult.Status)
                    return ServicesResult<string>.Failure($"Failed to retrieve user info: {userResult.Message}");

                // Log the update action
                var log = new ActivityLog
                {
                    Id = Guid.NewGuid().ToString(),
                    ProjectId = projectResult.Data.Id,
                    UserId = memberResult.Data.UserId,
                    ActionDate = DateTime.Now,
                    Action = $"Report updated by {userResult.Data.UserName} in plan {planResult.Data.Name}."
                };

                var logResponse = await _unitOfWork.ActivityLogRepository.AddAsync(log);
                if (!logResponse.Status)
                    return ServicesResult<string>.Failure($"Failed to log activity: {logResponse.Message}");

                return ServicesResult<string>.Success("Report updated successfully.");
            }
            catch (Exception ex)
            {
                return ServicesResult<string>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
        #endregion
        #region
        /// <summary>
        /// Deletes a progress report and logs the deletion action.
        /// </summary>
        /// <param name="memberId">The ID of the member performing the deletion.</param>
        /// <param name="reportId">The ID of the report to delete.</param>
        /// <returns>A service result indicating success or failure with a message.</returns>
        public async Task<ServicesResult<string>> DeleteReport(string memberId, string reportId)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(memberId) || string.IsNullOrEmpty(reportId))
                return ServicesResult<string>.Failure("Member ID and Report ID cannot be null or empty.");

            try
            {
                // Validate the member's role
                var memberRoleResult = await GetMemberRole();
                if (!memberRoleResult.Status)
                    return ServicesResult<string>.Failure($"Failed to validate member role: {memberRoleResult.Message}");

                // Retrieve the report to delete
                var reportResult = await _unitOfWork.ProgressReportRepository.GetOneByKeyAndValue("Id", reportId);
                if (!reportResult.Status)
                    return ServicesResult<string>.Failure($"Failed to retrieve report: {reportResult.Message}");

                // Retrieve the plan associated with the report
                var planResult = await _unitOfWork.PlanRepository.GetOneByKeyAndValue("Id", reportResult.Data.PlanId);
                if (!planResult.Status)
                    return ServicesResult<string>.Failure($"Failed to retrieve plan: {planResult.Message}");

                // Retrieve the project associated with the plan
                var projectResult = await _unitOfWork.ProjectRepository.GetOneByKeyAndValue("Id", planResult.Data.ProjectId);
                if (!projectResult.Status)
                    return ServicesResult<string>.Failure($"Failed to retrieve project: {projectResult.Message}");

                // Retrieve the member performing the deletion
                var memberResult = await _unitOfWork.ProjectMemberRepository.GetOneByKeyAndValue("Id", memberId);
                if (!memberResult.Status)
                    return ServicesResult<string>.Failure($"Failed to retrieve member: {memberResult.Message}");

                // Verify that the member belongs to the project and has the required role
                if (memberResult.Data.ProjectId != projectResult.Data.Id || memberResult.Data.RoleId != _memberId)
                    return ServicesResult<string>.Failure("Member does not have sufficient permissions to delete this report.");

                // Delete the report
                var deleteResult = await _unitOfWork.ProgressReportRepository.DeleteAsync(reportId);
                if (!deleteResult.Status)
                    return ServicesResult<string>.Failure($"Failed to delete report: {deleteResult.Message}");

                // Retrieve user info for logging purposes
                var userResult = await _unitOfWork.UserRepository.GetOneByKeyAndValue("Id", memberResult.Data.UserId);
                if (!userResult.Status)
                    return ServicesResult<string>.Failure($"Failed to retrieve user info: {userResult.Message}");

                // Log the deletion action
                var log = new ActivityLog
                {
                    Id = Guid.NewGuid().ToString(),
                    ProjectId = projectResult.Data.Id,
                    UserId = memberResult.Data.UserId,
                    ActionDate = DateTime.Now,
                    Action = $"Report deleted by {userResult.Data.UserName} in plan {planResult.Data.Name}."
                };

                var logResult = await _unitOfWork.ActivityLogRepository.AddAsync(log);
                if (!logResult.Status)
                    return ServicesResult<string>.Failure($"Failed to log activity: {logResult.Message}");

                return ServicesResult<string>.Success("Report deleted successfully.");
            }
            catch (Exception ex)
            {
                return ServicesResult<string>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
        #endregion

        #region Private method helper
        /// <summary>
        /// Gets the role ID by role name.
        /// </summary>
        /// <param name="roleName">The name of the role to fetch.</param>
        /// <returns>Service result indicating success or failure.</returns>
        private async Task<ServicesResult<bool>> GetRoleByName(string roleName)
        {
            try
            {
                var role = await _unitOfWork.RoleInProjectRepository.GetOneByKeyAndValue("Name", roleName);
                if (!role.Status)
                    return ServicesResult<bool>.Failure(role.Message);

                // Assign the role ID to the appropriate variable
                switch (roleName)
                {
                    case "Owner":
                        _ownerId = role.Data.Id;
                        break;
                    case "Leader":
                        _leaderId = role.Data.Id;
                        break;
                    case "Manager":
                        _managerId = role.Data.Id;
                        break;
                    case "Member":
                        _memberId = role.Data.Id;
                        break;
                    default:
                        return ServicesResult<bool>.Failure("Invalid role name");
                }

                return ServicesResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Failure(ex.Message);
            }
            finally
            {
                // Remove this if you don’t want to dispose the unit of work here.
                _unitOfWork.Dispose();
            }
        }

        /// <summary>
        /// Gets the role ID for the "Owner" role.
        /// </summary>
        /// <returns>Service result indicating success or failure.</returns>
        private async Task<ServicesResult<bool>> GetOwnRole()
        {
            return await GetRoleByName("Owner");
        }

        /// <summary>
        /// Gets the role ID for the "Leader" role.
        /// </summary>
        /// <returns>Service result indicating success or failure.</returns>
        private async Task<ServicesResult<bool>> GetLeaderRole()
        {
            return await GetRoleByName("Leader");
        }

        /// <summary>
        /// Gets the role ID for the "Manager" role.
        /// </summary>
        /// <returns>Service result indicating success or failure.</returns>
        private async Task<ServicesResult<bool>> GetManagerRole()
        {
            return await GetRoleByName("Manager");
        }

        /// <summary>
        /// Gets the role ID for the "Member" role.
        /// </summary>
        /// <returns>Service result indicating success or failure.</returns>
        private async Task<ServicesResult<bool>> GetMemberRole()
        {
            return await GetRoleByName("Member");
        }


        #endregion
    }
}
