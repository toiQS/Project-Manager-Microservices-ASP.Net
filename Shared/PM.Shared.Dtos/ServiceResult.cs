namespace PM.Shared.Dtos
{
    public class ServiceResult<T>
    {
        public string Message { get; set; } = string.Empty;
        public bool Status { get; set; } = true;
       
        public T? Data { get; set; }
        public DateTime ActionAt { get; set; }
        public ServiceResult(T? data)
        {
            Data = data;
            Status = true;
            Message = "Success";
            
            ActionAt = DateTime.UtcNow;
        }
        public static ServiceResult<T> Success(T data)
        {
            return new ServiceResult<T>(data);
        }
        public ServiceResult(Exception exception)
        {
            Data = default;
            Status = false;
            
            Message = exception.InnerException != null ? exception.InnerException.Message : exception.Message;
            ActionAt = DateTime.UtcNow;
        }
        public static ServiceResult<T> Failure(Exception exception)
        {
            return new ServiceResult<T>(exception);
        }

        public ServiceResult(string message)
        {
            Data = default;
            Status = false;
            
            Message = message;
            ActionAt = DateTime.UtcNow;
        }
        public static ServiceResult<T> Failure(string message)
        {
            return new ServiceResult<T>(message);
        }
    }
}
