namespace PM.Domain.Models.progressReports
{
    public class IndexRepost
    {
        public string Id { get; set; } = string.Empty;
        public string RepostDetail { get; set; } = string.Empty;
        public DateOnly RepostDate { get; set; }
    }
}
