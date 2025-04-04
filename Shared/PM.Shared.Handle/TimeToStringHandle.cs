namespace PM.Shared.Handle
{
    public class TimeToStringHandle
    {
        private readonly DateTime now = DateTime.UtcNow;
        public string TimeToString()
        {
            var time = now.ToString("dddd-dd-MM-yyyy-HH-mm-ss-ff-tt");
            return time;
        }
    }
}
