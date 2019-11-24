namespace QResurgence.SST.Messages
{
    internal class Request<T>
    {
        public Request(byte[] from, MessageType type, T payload)
        {
            From = from;
            Type = type;
            Payload = payload;
        }
        
        public byte[] From { get; }

        public MessageType Type { get; }

        public T Payload { get; }
    }
}