namespace QResurgence.SST.Messages
{
    /// <summary>
    ///     Implements the capability info
    /// </summary>
    public class CapabilityInfo
    {
        /// <summary>
        ///     Initializes an instance of the <see cref="CapabilityInfo" /> class
        /// </summary>
        /// <param name="name">The capability name</param>
        public CapabilityInfo(string name)
        {
            Name = name;
        }

        /// <summary>
        ///     The capability name
        /// </summary>
        public string Name { get; }
    }
}