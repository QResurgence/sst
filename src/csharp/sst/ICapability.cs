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
        /// <param name="argumentsJson">The JSON string representing the capability invocation arguments</param>
        /// <returns>The JSON string representing the capability invocation return value</returns>
        string Invoke(string argumentsJson);
    }
}