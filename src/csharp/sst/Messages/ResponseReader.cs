using System.Diagnostics;
using NetMQ;
using Newtonsoft.Json;
using QResurgence.SST.Security;
using QResurgence.SST.Utilities;

namespace QResurgence.SST.Messages
{
    internal static class ResponseReader
    {
        public static Response<object> Read(IDecryptor decryptor, NetMQMessage message)
        {
            return Read<object>(decryptor, message);
        }
        
        public static Response<T> Read<T>(IDecryptor decryptor, NetMQMessage message)
        {
            // Frame 0: Message type
            // Frame 1: Payload
            Debug.Assert(message.FrameCount == 2);

            var type = (MessageType) message.Pop().ConvertToInt32();

            var payload = default(T);
            if (!message.First.IsEmpty)
            {
                payload = JsonConvert.DeserializeObject<T>(decryptor.Decrypt(message.Pop().ToByteArray()));
            }
            
            return new Response<T>(type, payload);
        }

        public static Either<Response<ErrorCode>, Response<T>> Read<T>(IDecryptor decryptorLeft, IDecryptor decryptorRight, NetMQMessage message)
        {
            // Frame 0: Message type
            // Frame 1: Payload
            Debug.Assert(message.FrameCount == 2);
            
            var type = (MessageType) message.First.ConvertToInt32();

            return type == MessageType.Error
                ? (Either<Response<ErrorCode>, Response<T>>) new Left<Response<ErrorCode>, Response<T>>(
                    Read<ErrorCode>(decryptorLeft, message))
                : new Right<Response<ErrorCode>, Response<T>>(Read<T>(decryptorRight, message));
        }
    }
}