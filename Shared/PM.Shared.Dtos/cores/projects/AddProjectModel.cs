namespace PM.Shared.Dtos.cores.projects
{
    public class AddProjectModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public TypeStatus Status { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsModified { get; set; }
        public bool IsLocked { get; set; }
        public bool IsComplied { get; set; }
    }
}
