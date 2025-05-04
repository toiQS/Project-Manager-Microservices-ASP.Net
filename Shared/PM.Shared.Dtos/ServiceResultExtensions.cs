namespace PM.Shared.Dtos
{
    public static class ServiceResultExtensions
    {
        public static bool IsSuccess<T>(this ServiceResult<T> result)
        {
            return result.Status == ResultStatus.Success && result.Data != null;
        }
    }
}
