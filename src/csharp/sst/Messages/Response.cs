namespace QResurgence.SST.Messages
{
    internal class Response<T>
    {
        public Response(MessageType type, T payload)
        {
            Type = type;
            Payload = payload;
        }

        public MessageType Type { get; }

        public T Payload { get; }
    }
}