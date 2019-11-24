using System.Diagnostics;
using NetMQ;
using Newtonsoft.Json;
using QResurgence.SST.Security;

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
    }
}