using PM.Shared.Dtos.Enums;
using System.Text.Json.Serialization;

namespace PM.Shared.Dtos.Models
{
    public class ServiceResult<T>
    {
        [JsonConstructor]
        public ServiceResult(string message, string messageKey, ResultStatus status, T data, DateTime actionAt, object? metadata)
        {
            Message = message;
            MessageKey = messageKey;
            Status = status;
            Data = data;
            ActionAt = actionAt;
            Metadata = metadata;
        }

        // Constructor mặc định thêm vào để tiện tạo object khi cần (không ảnh hưởng JSON deserialization)
        public ServiceResult()
        {
            ActionAt = DateTime.UtcNow;
        }

        public string Message { get; set; }
        public string MessageKey { get; set; }
        public ResultStatus Status { get; set; }
        public T Data { get; set; }
        public DateTime ActionAt { get; set; }
        public object? Metadata { get; set; }

        // Factory method: Success
        public static ServiceResult<T> Success(T data)
        {
            return new ServiceResult<T>
            {
                Status = ResultStatus.Success,
                Data = data,
                ActionAt = DateTime.UtcNow
            };
        }

        // Factory method: Error
        public static ServiceResult<T> Error(string message)
        {
            return new ServiceResult<T>
            {
                Status = ResultStatus.Error,
                Message = message,
                ActionAt = DateTime.UtcNow
            };
        }

        // Factory method: From Exception
        public static ServiceResult<T> FromException(Exception ex)
        {
            return new ServiceResult<T>
            {
                Status = ResultStatus.Error,
                Message = ex.Message,
                Metadata = ex.StackTrace,
                ActionAt = DateTime.UtcNow
            };
        }
    }

    // Enum định nghĩa các trạng thái kết quả
    public enum ResultStatus
    {
        Success = 1,
        Error = 3,
        Warning = 2
        // Bạn có thể mở rộng thêm: Unauthorized, Forbidden, NotFound...
    }
}
