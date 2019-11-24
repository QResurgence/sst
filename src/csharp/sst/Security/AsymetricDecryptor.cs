using System.Security.Cryptography;
using System.Text;

namespace QResurgence.SST.Security
{
    internal class AsymetricDecryptor : IDecryptor
    {
        private readonly RSACryptoServiceProvider _rsaProvider;

        public AsymetricDecryptor(string publicKey)
        {
            _rsaProvider = new RSACryptoServiceProvider();
            _rsaProvider.FromXmlString(publicKey);
        }
        
        public string Decrypt(byte[] payload)
        {
            return Encoding.UTF8.GetString(_rsaProvider.Decrypt(payload, fOAEP: false));
        }
    }
}