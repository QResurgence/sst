using NetMQ;

namespace QResurgence.SST.Messages
{
    internal static class ResponseCreator
    {
        public static NetMQMessage Create(byte[] to, MessageType type)
        {
            return Create(to, type, new byte[]{});
        }
        
        public static NetMQMessage Create(byte[] to, MessageType type, byte[] payload)
        {
            var response = new NetMQMessage();
            response.Append(to);
            response.AppendEmptyFrame();
            response.Append((int)type);

            if (payload.Length == 0)
            {
                response.AppendEmptyFrame();
            }
            else
            {
                response.Append(payload);
            }
            
            return response;
        }
    }
}