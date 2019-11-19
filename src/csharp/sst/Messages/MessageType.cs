namespace QResurgence.SST.Messages
{
    internal enum MessageType
    {
        None = 0,
        RequestPublicKey = 1,
        SendPublicKey = 2,
        SendEncryptionKey = 3,
        SendChallenge = 4,
        ChallengeResponse = 5,
        Acknowledge = 6,
        RegisterCapability = 7,
        UnregisterCapability = 8,
        GetCapability = 9,
        GrantCapability = 10,
        InvokeCapability = 11,
        CapabilityInvocationResult = 12,
        Error = 13
    }
}