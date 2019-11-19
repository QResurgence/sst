namespace QResurgence.SST.Security
{
    internal class EncryptionKey
    {
        public EncryptionKey(byte[] key, byte[] nonce)
        {
            Key = key;
            Nonce = nonce;
        }

        public byte[] Key { get; }

        public byte[] Nonce { get; }
    }
}