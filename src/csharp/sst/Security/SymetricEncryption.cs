using System;
using System.Linq;

namespace QResurgence.SST.Security
{
    internal class SymetricEncryption : IEncryptor, IDecryptor
    {
        private readonly ChaCha20 _encryptor;

        public SymetricEncryption()
        {
            EncryptionKey = new EncryptionKey(GenerateEncryptionKey(), GenerateNonce());
            _encryptor = new ChaCha20(EncryptionKey.Key, EncryptionKey.Nonce, 1);
        }

        public SymetricEncryption(EncryptionKey key)
        {
            EncryptionKey = key;
            _encryptor = new ChaCha20(EncryptionKey.Key, EncryptionKey.Nonce, 1);
        }

        public EncryptionKey EncryptionKey { get; }

        public string Decrypt(byte[] payload)
        {
            return _encryptor.DecryptUTF8ByteArray(payload);
        }

        public byte[] Encrypt(string payloadJson)
        {
            return _encryptor.EncryptString(payloadJson);
        }

        private static byte[] GenerateEncryptionKey()
        {
            return Guid.NewGuid().ToByteArray()
                .Aggregate(Guid.NewGuid().ToByteArray(), (current, b) => current.Append(b).ToArray());
        }

        private static byte[] GenerateNonce()
        {
            return Guid.NewGuid().ToByteArray().Take(12).ToArray();
        }
    }
}