namespace PM.Application.Features.Projects.Command
{
    public class AddProjectCommand
    {
        public string Token { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectDescription { get; set; } = string.Empty;
        public DateOnly StartAt { get; set; }
        public DateOnly EndAt { get; set; }
    }
}