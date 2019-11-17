namespace QResurgence.SST.Messages
{
    /// <summary>
    ///     Implements the Error message
    /// </summary>
    public class ErrorMessage
    {
        /// <summary>
        ///     Initializes an instance of the <see cref="ErrorMessage" /> class
        /// </summary>
        /// <param name="errorCode">The error code</param>
        public ErrorMessage(ErrorCode errorCode)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        ///     The error code
        /// </summary>
        public ErrorCode ErrorCode { get; }
    }
}