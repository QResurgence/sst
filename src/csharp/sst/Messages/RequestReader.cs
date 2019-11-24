using System.Diagnostics;
using NetMQ;
using Newtonsoft.Json;
using QResurgence.SST.Security;

namespace QResurgence.SST.Messages
{
    internal static class RequestReader
    {
        public static Request<object> Read(IDecryptor decryptor, NetMQMessage message)
        {
            return Read<object>(decryptor, message);
        }
        
        public static Request<T> Read<T>(IDecryptor decryptor, NetMQMessage message)
        {
            // Frame 0: RequesterID
            // Frame 1: Empty
            // Frame 2: Message type
            // Frame 3: Payload
            Debug.Assert(message.FrameCount == 4);

            var requester = message.Pop().ToByteArray();
            message.Pop(); // Empty frame
            var type = (MessageType) message.Pop().ConvertToInt32();

            var payload = default(T);
            if (!message.First.IsEmpty)
            {
                payload = JsonConvert.DeserializeObject<T>(decryptor.Decrypt(message.Pop().ToByteArray()));
            }

            return new Request<T>(requester, type, payload);
        }
    }
}