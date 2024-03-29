using Newtonsoft.Json;

namespace QResurgence.SST.Capability
{
    /// <summary>
    ///     Implements the capability interface and defines the invocation argument and return value types
    /// </summary>
    /// <typeparam name="TArguments">The invocation argument type</typeparam>
    /// <typeparam name="TReturn">The return value type</typeparam>
    public abstract class BaseCapability<TArguments, TReturn> : ICapability
    {
        /// <inheritdoc />
        public string Invoke(string argumentJson)
        {
            return Serialize(Invoke(Deserialize(argumentJson)));
        }

        /// <summary>
        ///     Invokes the capability with correct types
        /// </summary>
        /// <param name="argument">The invocation argument</param>
        /// <returns>The return value</returns>
        protected abstract TReturn Invoke(TArguments argument);

        private static TArguments Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<TArguments>(json);
        }

        private static string Serialize(TReturn r)
        {
            return JsonConvert.SerializeObject(r);
        }
    }
}