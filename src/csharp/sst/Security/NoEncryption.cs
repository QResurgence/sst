using System.Text;

namespace QResurgence.SST.Security
{
    internal class NoEncryption : IEncryptor, IDecryptor
    {
        public byte[] Encrypt(string payloadJson)
        {
            return Encoding.UTF8.GetBytes(payloadJson);
        }

        public string Decrypt(byte[] payload)
        {
            return Encoding.UTF8.GetString(payload);
        }
    }
}