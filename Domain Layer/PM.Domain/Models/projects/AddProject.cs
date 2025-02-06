namespace PM.Domain.Models.projects
{
    public class AddProject
    {
        public string ProjectName { get; set; } = string.Empty;
        public DateTime StartAt { get; set; } 
        public DateTime EndAt { get; set; } 
        public string ProjectDescription { get; set; } = string.Empty;
    }
}
