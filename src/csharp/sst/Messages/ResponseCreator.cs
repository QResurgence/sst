using NetMQ;
using QResurgence.SST.Security;

namespace QResurgence.SST.Messages
{
    internal static class ResponseCreator
    {
        public static NetMQMessage Create(IEncryptor encryptor, byte[] to, MessageType type)
        {
            return Create(encryptor, to, type, string.Empty);
        }
        
        public static NetMQMessage Create(IEncryptor encryptor, byte[] to, MessageType type, string payloadJson)
        {
            var response = new NetMQMessage();
            response.Append(to);
            response.AppendEmptyFrame();
            response.Append((int)type);

            if (string.IsNullOrEmpty(payloadJson))
            {
                response.AppendEmptyFrame();
            }
            else
            {
                response.Append(encryptor.Encrypt(payloadJson));
            }
            
            return response;
        }
    }
}