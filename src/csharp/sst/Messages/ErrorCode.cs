namespace QResurgence.SST.Messages
{
    /// <summary>
    ///     Implement the error codes
    /// </summary>
    internal enum ErrorCode
    {
        /// <summary>
        ///     Request denied error code
        /// </summary>
        RequestDenied = 0,

        /// <summary>
        ///     Invocation error error code
        /// </summary>
        InvocationError = 1,
        ChallengeFailed,
        EncryptionKeyAlreadySent
    }
}