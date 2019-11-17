namespace QResurgence.SST
{
    /// <summary>
    ///     Defines the capability interface
    /// </summary>
    public interface ICapability : ICapabilityDefinition
    {
        /// <summary>
        ///     Invokes the capability
        /// </summary>
        /// <param name="arguments">The byte array of the JSON string representing the capability invocation arguments</param>
        /// <returns>The byte array of the JSON string representing the capability invocation return value</returns>
        byte[] Invoke(byte[] arguments);
    }
}