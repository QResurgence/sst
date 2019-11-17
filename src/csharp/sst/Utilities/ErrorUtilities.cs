using System;
using QResurgence.SST.Errors;
using QResurgence.SST.Messages;

namespace QResurgence.SST.Utilities
{
    internal static class ErrorUtilities
    {
        public static IError GetErrorByErrorCode(ErrorCode errorCode)
        {
            switch (errorCode)
            {
                case ErrorCode.RequestDenied:
                    return new RequestDeniedError();
                case ErrorCode.InvocationError:
                    return new CapabilityInvocationError();
                default:
                    throw new ArgumentOutOfRangeException(nameof(errorCode), errorCode, null);
            }
        }
    }
}