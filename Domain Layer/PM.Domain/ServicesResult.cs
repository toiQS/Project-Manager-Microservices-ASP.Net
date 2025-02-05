namespace PM.Domain
{
    public class ServicesResult<T>
    {
        #region Properties

        /// <summary>
        /// Gets the message providing additional context about the result.
        /// </summary>
        public string Message { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the data associated with the result, if any.
        /// </summary>
        public T? Data { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool Status { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ServicesResult() { }

        /// <summary>
        /// Constructor for creating a failure result with a message.
        /// </summary>
        /// <param name="message">The failure message.</param>
        public ServicesResult(string message)
        {
            Message = message;
            Status = false;
        }

        /// <summary>
        /// Constructor for creating a success result with data.
        /// </summary>
        /// <param name="data">The data associated with the success result.</param>
        public ServicesResult(T data, string? message)
        {
            if (data is null)
            {
                Status = true;
                Message = $"No data available, {message}";
            }
            else
            {
                Data = data;
                Status = true;
                Message = "Success";
            }
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Creates a success result with the specified data.
        /// </summary>
        /// <param name="data">The data to include in the result.</param>
        /// <returns>A success <see cref="ServicesResult{T}"/> instance.</returns>
        public static ServicesResult<T> Success(T data, string message)
        {
            return new ServicesResult<T>(data, message);
        }

        /// <summary>
        /// Creates a failure result with the specified message.
        /// </summary>
        /// <param name="message">The failure message.</param>
        /// <returns>A failure <see cref="ServicesResult{T}"/> instance.</returns>
        public static ServicesResult<T> Failure(string message)
        {
            return new ServicesResult<T>(message);
        }

        #endregion
    }
}
