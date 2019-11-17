using QResurgence.SST.Attribute;
using QResurgence.SST.Capability;

namespace QResurgence.SST.example
{
    [Capability("Addition")]
    internal class AdditionCapability : BaseCapability<AdditionArguments, AdditionReturnValue>, IAdditionCapability
    {
        protected override AdditionReturnValue Invoke(AdditionArguments arguments) =>
            new AdditionReturnValue(arguments.Left + arguments.Right);
    }
}