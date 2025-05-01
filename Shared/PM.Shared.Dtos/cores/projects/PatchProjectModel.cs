namespace PM.Shared.Dtos.cores.projects
{
    public class PatchProjectModel
    {
        public string Id { get; set; } = string.Empty;  
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly StartAt { get; set; }
        public DateOnly EndAt { get; set; }
        public TypeStatus Status { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsModified { get; set; }
        public bool IsLocked { get; set; }
        public bool IsComplied { get; set; }
    }
}
