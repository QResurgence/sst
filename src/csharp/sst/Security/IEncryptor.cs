namespace QResurgence.SST.Security
{
    internal interface IEncryptor
    {
        byte[] Encrypt(string payloadJson);
    }
}