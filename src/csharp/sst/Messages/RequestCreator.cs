using NetMQ;
using QResurgence.SST.Security;

namespace QResurgence.SST.Messages
{
    internal static class RequestCreator
    {
        public static NetMQMessage Create(IEncryptor encryptor, MessageType type)
        {
            return Create(encryptor, type, string.Empty);
        }

        public static NetMQMessage Create(IEncryptor encryptor, MessageType type, string payloadJson)
        {
            var request = new NetMQMessage();
            request.Append((int) type);

            if (string.IsNullOrEmpty(payloadJson))
                request.AppendEmptyFrame();
            else
                request.Append(encryptor.Encrypt(payloadJson));

            return request;
        }
    }
}