namespace PM.Domain.Models.projects
{
    public class UpdateProject
    {
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectDescription { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; } // Ngày bắt đầu
        public DateOnly EndDate { get; set; } // Ngày kết thúc

    }
}
