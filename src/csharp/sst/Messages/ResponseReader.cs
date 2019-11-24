using System.Diagnostics;
using NetMQ;
using Newtonsoft.Json;

namespace QResurgence.SST.Messages
{
    internal static class ResponseReader
    {
        public static Response<object> Read(NetMQMessage message)
        {
            return Read<object>(message);
        }
        
        public static Response<T> Read<T>(NetMQMessage message)
        {
            // Frame 0: Message type
            // Frame 1: Payload
            Debug.Assert(message.FrameCount == 2);

            var type = (MessageType) message.Pop().ConvertToInt32();

            var payload = default(T);
            if (!message.First.IsEmpty)
            {
                payload = JsonConvert.DeserializeObject<T>(message.Pop().ConvertToString());
            }
            
            return new Response<T>(type, payload);
        }
    }
}