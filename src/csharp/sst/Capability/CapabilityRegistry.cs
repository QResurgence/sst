using System.Collections.Generic;
using QResurgence.SST.Utilities;

namespace QResurgence.SST.Capability
{
    internal class CapabilityRegistry
    {
        private readonly Dictionary<string, ICapability> _capabilities;

        public CapabilityRegistry()
        {
            _capabilities = new Dictionary<string, ICapability>();
        }

        public bool Contains(string name)
        {
            return _capabilities.ContainsKey(name);
        }

        public void Add<TCapability>(string name, TCapability capability)
            where TCapability : ICapability, new()
        {
            _capabilities.Add(name, capability);
        }

        public Maybe<ICapability> Get(string name)
        {
            return _capabilities.ContainsKey(name)
                ? new Maybe<ICapability>(_capabilities[name])
                : new Nothing<ICapability>();
        }
    }
}