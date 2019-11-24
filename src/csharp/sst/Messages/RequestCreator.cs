using NetMQ;

namespace QResurgence.SST.Messages
{
    internal static class RequestCreator
    {
        public static NetMQMessage Create(MessageType type)
        {
            return Create(type, new byte[] { });
        }
        
        public static NetMQMessage Create(MessageType type, byte[] payload)
        {
            var request = new NetMQMessage();
            request.Append((int)type);

            if (payload.Length == 0)
            {
                request.AppendEmptyFrame();
            }
            else
            {
                request.Append(payload);
            }

            return request;
        }
    }
}