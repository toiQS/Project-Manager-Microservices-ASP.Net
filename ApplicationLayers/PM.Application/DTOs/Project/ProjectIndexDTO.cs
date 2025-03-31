namespace PM.Application.DTOs.Project
{
    public class ProjectIndexDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        
        public DateTime CreatedDate { get; set; }

        public string OwnerName { get; set; } = string.Empty.ToString();

    }
}
