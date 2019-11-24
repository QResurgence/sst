using NetMQ;
using Newtonsoft.Json;
using QResurgence.SST.Messages;
using QResurgence.SST.Security;
using ErrorCode = QResurgence.SST.Messages.ErrorCode;

namespace QResurgence.SST.Server
{
    internal static class ErrorMessageSender
    {
        public static void SendError(byte[] destination, NetMQSocket socket, ErrorCode errorCode)
        {
            var response = ResponseCreator.Create(new NoEncryption(), destination, MessageType.Error, JsonConvert.SerializeObject(errorCode));

            socket.SendMultipartMessage(response);
        }
    }
}