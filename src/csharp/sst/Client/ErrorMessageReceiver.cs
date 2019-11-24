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
            return error switch
            {
                ErrorCode.RequestDenied => (IError) new RequestDeniedError(),
                ErrorCode.InvocationError => new CapabilityInvocationError(),
                _ => new UnknownError()
            };
        }
    }
}