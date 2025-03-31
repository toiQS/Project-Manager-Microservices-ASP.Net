namespace PM.Application.Features.Projects.Command
{
    public class PatchProjectCommand
    {
        public string UserId { get; set; } = string.Empty;
        public string ProjectId { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectDescription { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool IsDelete { get; set; }
        public bool IsComplete { get; set; }
    }
}
