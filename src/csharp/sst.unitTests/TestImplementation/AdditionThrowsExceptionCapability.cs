using System;
using QResurgence.SST.Attribute;
using QResurgence.SST.Capability;

namespace QResurgence.SST.UnitTests.TestImplementation
{
    [Capability("Addition")]
    internal class AdditionThrowsExceptionCapability : BaseCapability<AdditionArguments, AdditionReturnValue>, IAdditionCapability
    {
        protected override AdditionReturnValue Invoke(AdditionArguments arguments) => throw new Exception("Don't know how to calculate.");
    }
}