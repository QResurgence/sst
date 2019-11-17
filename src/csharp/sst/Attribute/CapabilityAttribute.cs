using System;

namespace QResurgence.SST.Attribute
{
    /// <summary>
    ///     Defines the attribute used to give a name to the capability. Must be the same on the capability definition
    ///     interface an on the implementation of the capability
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class CapabilityAttribute : System.Attribute
    {
        /// <summary>
        ///     Initializes an instance of the <see cref="CapabilityAttribute" /> class
        /// </summary>
        /// <param name="name">The name of the capability</param>
        public CapabilityAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        ///     The name of the capability
        /// </summary>
        public string Name { get; }
    }
}