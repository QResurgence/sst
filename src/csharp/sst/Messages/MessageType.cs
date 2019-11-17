namespace QResurgence.SST.Messages
{
    /// <summary>
    ///     Implements the message types
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        ///     Placeholder message type
        /// </summary>
        None = 0,

        /// <summary>
        ///     The request public key (sent by the server and client to the Capability Registry Server and by the client to the
        ///     server)
        /// </summary>
        RequestPublicKey = 1,

        /// <summary>
        ///     The send public key (sent by the Capability Registry Server to the server and client and by the server to the
        ///     client)
        /// </summary>
        SendPublicKey = 2,

        /// <summary>
        ///     The send encryption key (sent by the server and client to the Capability Registry Server and by the client to the
        ///     server)
        /// </summary>
        SendEncryptionKey = 3,

        /// <summary>
        ///     The send challenge (sent by the Capability Registry Server to the server and client and by the server to the
        ///     client)
        /// </summary>
        SendChallenge = 4,

        /// <summary>
        ///     The challenge response (sent by the server and client to the Capability Registry Server and by the client to the
        ///     server)
        /// </summary>
        ChallengeResponse = 5,

        /// <summary>
        ///     The acknowledge (sent by the Capability Registry Server to the server and client and by the server to the client)
        /// </summary>
        Acknowledge = 6,

        /// <summary>
        ///     The register capability message type (used by the service to register the capability with the Capability Registry
        ///     Server)
        /// </summary>
        RegisterCapability = 7,

        /// <summary>
        ///     The unregister capability message type (used by the service to unregister the capability with the Capability
        ///     Registry Server)
        /// </summary>
        UnregisterCapability = 8,

        /// <summary>
        ///     The get capability message type (sent by the client to Capability Registry Service and the server)
        /// </summary>
        GetCapability = 9,

        /// <summary>
        ///     The grant capability (sent by the Capability Registry Service and the server to the client)
        /// </summary>
        GrantCapability = 10,

        /// <summary>
        ///     The invoke capability (sent by the client to the service)
        /// </summary>
        InvokeCapability = 11,

        /// <summary>
        ///     The capability invocation result (sent by the service to the client)
        /// </summary>
        CapabilityInvocationResult = 12,

        /// <summary>
        ///     The error (sent by the Capability Registry Service and the server to the client and by the Capability Registration
        ///     Server to the server)
        /// </summary>
        Error = 13
    }
}