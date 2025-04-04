namespace PM.Shared.Dtos
{
    public class ContentLog
    {
        public string ActionName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool Status { get; set; } = true;   
        public int StatusCode { get; set; } = 200;
        public DateTime ActionAt { get; set; } = DateTime.UtcNow;
    }
}
