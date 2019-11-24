using System.Text;

namespace QResurgence.SST.Security
{
    internal class NoEncryption : IEncryptor, IDecryptor
    {
        public string Decrypt(byte[] payload)
        {
            return Encoding.UTF8.GetString(payload);
        }

        public byte[] Encrypt(string payloadJson)
        {
            return Encoding.UTF8.GetBytes(payloadJson);
        }
    }
}