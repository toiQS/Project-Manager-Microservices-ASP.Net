namespace PM.Domain.Models.documents
{
    public class AddDoc
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string ProjectId { get; set; }
        public string PlanId { get; set; }
        public string MissionId { get; set; }
    }
}
