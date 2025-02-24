namespace PM.Application.Models.projects
{
    public class UpdateProjectModel
    {
        public string Token { get; set; } = string.Empty;
        public string ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectDescription { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
