using System.Text;
using Newtonsoft.Json;
using QResurgence.SST.Errors;
using QResurgence.SST.Messages;

namespace QResurgence.SST.Client
{
    internal static class ErrorMessageReceiver
    {
        public static IError GetError(byte[] errorContent)
        {
            var error = JsonConvert.DeserializeObject<ErrorCode>(Encoding.UTF8.GetString(errorContent));
            switch (error)
            {
                case ErrorCode.RequestDenied:
                    return new RequestDeniedError();
                case ErrorCode.InvocationError:
                    return new CapabilityInvocationError();
                default:
                    return new UnknownError();
            }
        }
    }
}