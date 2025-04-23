namespace PM.Shared.Dtos
{
    public enum ResultStatus
    {
        Success,
        Info,
        Warning,
        Error
    }

    public class ServiceResult<T>
    {
        public string Message { get; set; } = string.Empty;
        public string? MessageKey { get; set; }
        public ResultStatus Status { get; set; } = ResultStatus.Success;
        public T? Data { get; set; }
        public DateTime ActionAt { get; set; } = DateTime.UtcNow;
        public object? Metadata { get; set; }

        private ServiceResult() { }

        public static ServiceResult<T> Success(T data, string message = "Success") =>
            new ServiceResult<T> { Data = data, Status = ResultStatus.Success, Message = message };

        public static ServiceResult<T> Error(string message, string? messageKey = null) =>
            new ServiceResult<T> { Status = ResultStatus.Error, Message = message, MessageKey = messageKey };

        public static ServiceResult<T> FromException(Exception ex) =>
            new ServiceResult<T>
            {
                Status = ResultStatus.Error,
                Message = ex.InnerException?.Message ?? ex.Message
            };
    }
}