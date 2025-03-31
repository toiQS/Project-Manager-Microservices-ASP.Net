namespace PM.Application.DTOs.Project
{
    public class ProjectDetailDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;    
        public string Description { get; set; } = string.Empty;
        public string StatusInfo {  get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } 

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        public string DeleteInfo { get; set; } = string.Empty;
        public string CompleteInfo { get; set; } = string.Empty;
    }
}
