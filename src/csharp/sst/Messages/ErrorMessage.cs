namespace QResurgence.SST.Messages
{
    internal class ErrorMessage
    {
        public ErrorMessage(ErrorCode errorCode)
        {
            ErrorCode = errorCode;
        }

        public ErrorCode ErrorCode { get; }
    }
}