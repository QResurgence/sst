using System.Security.Cryptography;
using System.Text;

namespace QResurgence.SST.Security
{
    internal class AsymetricEncryptor : IEncryptor
    {
        private readonly RSACryptoServiceProvider _rsaProvider;

        public AsymetricEncryptor(string publicKey)
        {
            _rsaProvider = new RSACryptoServiceProvider();
            _rsaProvider.FromXmlString(publicKey);
        }

        public byte[] Encrypt(string payloadJson)
        {
            return _rsaProvider.Encrypt(Encoding.UTF8.GetBytes(payloadJson), false);
        }
    }
}