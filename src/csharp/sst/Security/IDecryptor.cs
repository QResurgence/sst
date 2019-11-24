namespace QResurgence.SST.Security
{
    internal interface IDecryptor
    {
        string Decrypt(byte[] payload);
    }
}