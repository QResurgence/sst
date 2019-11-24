using System.Linq;
using QResurgence.SST.Attribute;

namespace QResurgence.SST.Utilities
{
    internal static class CapabilityUtilities
    {
        public static string GetCapabilityName<TICapability>() where TICapability : ICapabilityDefinition
        {
            return (typeof(TICapability)
                .GetCustomAttributes(typeof(CapabilityAttribute), true)
                .FirstOrDefault() as CapabilityAttribute)?.Name;
        }
    }
}