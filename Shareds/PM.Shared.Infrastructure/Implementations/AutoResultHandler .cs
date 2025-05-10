using PM.Shared.Dtos.Enums;
using PM.Shared.Dtos.Models;
using System.Collections;

namespace PM.Shared.Infrastructure.Implementations
{
    public static class AutoResultHandler
    {
        //public static async Task<ServiceResult<T>> ExecuteAsync<T>(Func<Task<T>> action, string? validationErrorMessage = null)
        //{
        //    try
        //    {
        //        // Kiểm tra đầu vào null hoặc không hợp lệ
        //        if (!string.IsNullOrWhiteSpace(validationErrorMessage))
        //        {
        //            return new ServiceResult<T>
        //            {
        //                Status = ResultStatus.Error,
        //                Message = validationErrorMessage,
        //                ActionAt = DateTime.UtcNow
        //            };
        //        }

        //        T result = await action();
        //        return Evaluate(result);
        //    }
        //    catch (ArgumentNullException ex)
        //    {
        //        return new ServiceResult<T>
        //        {
        //            Status = ResultStatus.Error,
        //            Message = "Giá trị đầu vào không hợp lệ (null).",
        //            Metadata = ex.StackTrace,
        //            ActionAt = DateTime.UtcNow
        //        };
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return new ServiceResult<T>
        //        {
        //            Status = ResultStatus.Error,
        //            Message = "Giá trị đầu vào không hợp lệ.",
        //            Metadata = ex.StackTrace,
        //            ActionAt = DateTime.UtcNow
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResult<T>
        //        {
        //            Status = ResultStatus.Error,
        //            Message = ex.Message,
        //            Metadata = ex.StackTrace,
        //            ActionAt = DateTime.UtcNow
        //        };
        //    }
        //}

        //private static ServiceResult<T> Evaluate<T>(T result)
        //{
        //    var now = DateTime.UtcNow;

        //    // Kiểm tra nếu kết quả trả về null
        //    if (result == null)
        //    {
        //        return new ServiceResult<T>
        //        {
        //            Status = ResultStatus.NotFound,
        //            Message = "Không tìm thấy dữ liệu.",
        //            ActionAt = now
        //        };
        //    }

        //    // Kiểm tra nếu kết quả là danh sách rỗng
        //    if (result is IEnumerable enumerable)
        //    {
        //        bool isEmpty = !enumerable.Cast<object>().Any();
        //        if (isEmpty)
        //        {
        //            return new ServiceResult<T>
        //            {
        //                Status = ResultStatus.Info,
        //                Message = "Danh sách rỗng.",
        //                Data = result,
        //                ActionAt = now
        //            };
        //        }
        //    }

        //    // Trả về kết quả thành công
        //    return new ServiceResult<T>
        //    {
        //        Status = ResultStatus.Success,
        //        Message = "Thành công.",
        //        Data = result,
        //        ActionAt = now
        //    };
        //}
    }
}
