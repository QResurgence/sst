using QResurgence.SST.Attribute;
using QResurgence.SST.Capability;

namespace QResurgence.SST.UnitTests.TestImplementation
{
    [Capability("Addition")]
    internal class AdditionCapability : BaseCapability<AdditionArguments, AdditionReturnValue>, IAdditionCapability
    {
        protected override AdditionReturnValue Invoke(AdditionArguments arguments)
        {
            return new AdditionReturnValue(arguments.Left + arguments.Right);
        }
    }
}