using System;

namespace QResurgence.SST
{
    /// <summary>
    ///     Defines the server interface
    /// </summary>
    public interface IServer : IDisposable
    {
        /// <summary>
        ///     Starts listening for requests
        /// </summary>
        void Serve();

        /// <summary>
        ///     Register a capability of <typeparamref name="TCapability" />
        /// </summary>
        /// <typeparam name="TCapability">The type implementing the capability</typeparam>
        void Register<TCapability>() where TCapability : ICapability, new();
    }
}