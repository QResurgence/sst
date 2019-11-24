using System.Security.Cryptography;
using System.Text;

namespace QResurgence.SST.Security
{
    internal class AsymetricEncryptionClient : IEncryptor, IDecryptor
    {
        private readonly RSACryptoServiceProvider _rsaProvider;

        public AsymetricEncryptionClient(string publicKey)
        {
            _rsaProvider = new RSACryptoServiceProvider();
            _rsaProvider.FromXmlString(publicKey);
        }
        
        public string Decrypt(byte[] payload)
        {
            return Encoding.UTF8.GetString(_rsaProvider.Decrypt(payload, fOAEP: false));
        }

        public byte[] Encrypt(string payloadJson)
        {
            return _rsaProvider.Encrypt(Encoding.UTF8.GetBytes(payloadJson), fOAEP: false);
        }
    }
}