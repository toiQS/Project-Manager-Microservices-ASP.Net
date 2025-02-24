namespace PM.Domain.Models.projects
{
    public class AddProject
    {
        public string ProjectName { get; set; } = string.Empty;
        public DateOnly StartAt { get; set; } 
        public DateOnly EndAt { get; set; } 
        public string ProjectDescription { get; set; } = string.Empty;
    }
}
