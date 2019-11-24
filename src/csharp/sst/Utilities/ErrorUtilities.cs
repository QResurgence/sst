using System;
using QResurgence.SST.Errors;
using QResurgence.SST.Messages;

namespace QResurgence.SST.Utilities
{
    internal static class ErrorUtilities
    {
        public static IError GetErrorByErrorCode(ErrorCode errorCode)
        {
            return errorCode switch
            {
                ErrorCode.RequestDenied => (IError) new RequestDeniedError(),
                ErrorCode.InvocationError => new CapabilityInvocationError(),
                _ => throw new ArgumentOutOfRangeException(nameof(errorCode), errorCode, null)
            };
        }
    }
}