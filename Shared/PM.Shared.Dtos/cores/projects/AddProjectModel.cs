namespace PM.Shared.Dtos.cores.projects
{
    public class AddProjectModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly StartAt { get; set; }
        public DateOnly EndAt { get; set; }
    }
}
