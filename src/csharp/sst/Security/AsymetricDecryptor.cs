using System.Security.Cryptography;
using System.Text;

namespace QResurgence.SST.Security
{
    internal class AsymetricDecryptor : IDecryptor
    {
        private readonly RSACryptoServiceProvider _rsaProvider;

        public AsymetricDecryptor()
        {
            _rsaProvider = new RSACryptoServiceProvider();
            PublicKey = _rsaProvider.ToXmlString(false);
            var privateKey = _rsaProvider.ToXmlString(true);

            _rsaProvider.FromXmlString(privateKey);
        }

        public string PublicKey { get; }

        public string Decrypt(byte[] payload)
        {
            return Encoding.UTF8.GetString(_rsaProvider.Decrypt(payload, false));
        }
    }
}