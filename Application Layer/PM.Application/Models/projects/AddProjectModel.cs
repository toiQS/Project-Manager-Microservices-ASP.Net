namespace PM.Application.Models.projects
{
  public  class AddProjectModel
    {
        public string Token { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectDescription { get; set; } = string.Empty;
        public DateOnly StartAt { get; set; }
        public DateOnly EndAt { get; set; }
    }
}
