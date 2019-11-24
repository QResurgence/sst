using System.Security.Cryptography;
using System.Text;

namespace QResurgence.SST.Security
{
    internal class AsymetricEncryptor : IEncryptor
    {
        private readonly RSACryptoServiceProvider _rsaProvider;

        public AsymetricEncryptor()
        {
            _rsaProvider = new RSACryptoServiceProvider();
            PublicKey = _rsaProvider.ToXmlString(false);
            var privateKey = _rsaProvider.ToXmlString(true);

            _rsaProvider.FromXmlString(privateKey);
        }

        public string PublicKey { get; }
        
        public byte[] Encrypt(string payloadJson)
        {
            return _rsaProvider.Encrypt(Encoding.UTF8.GetBytes(payloadJson), fOAEP: false);
        }
    }
}