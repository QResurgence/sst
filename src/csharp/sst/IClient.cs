using QResurgence.SST.Capability;
using QResurgence.SST.Errors;
using QResurgence.SST.Utilities;

namespace QResurgence.SST
{
    /// <summary>
    ///     Defines the client interface
    /// </summary>
    public interface IClient
    {
        /// <summary>
        ///     Gets the capability for use by the client defined by the <typeparamref name="TICapability" /> interface type,
        ///     accepting the <typeparamref name="TArgument" /> type invocation argument and returning
        ///     <typeparamref name="TReturn" /> type value
        /// </summary>
        /// <typeparam name="TICapability">The capability definition type</typeparam>
        /// <typeparam name="TArgument">The invocation argument type</typeparam>
        /// <typeparam name="TReturn">The return value type</typeparam>
        /// <returns>Returns the capability client or an error</returns>
        Either<IError, CapabilityClient<TArgument, TReturn>> GetCapability<TICapability, TArgument, TReturn>()
            where TICapability : ICapabilityDefinition;
    }
}