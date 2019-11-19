using NetMQ;
using Newtonsoft.Json;
using QResurgence.SST.Messages;
using ErrorCode = QResurgence.SST.Messages.ErrorCode;

namespace QResurgence.SST.Server
{
    internal static class ErrorMessageSender
    {
        public static void SendError(byte[] destination, NetMQSocket socket, ErrorCode errorCode)
        {
            var response = new NetMQMessage();
            response.Append(destination);
            response.AppendEmptyFrame();
            response.Append((int) MessageType.Error);
            response.Append(JsonConvert.SerializeObject(new ErrorMessage(errorCode)));

            socket.SendMultipartMessage(response);
        }
    }
}