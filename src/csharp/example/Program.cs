using System;
using QResurgence.SST.Errors;
using QResurgence.SST.Extensions;
using QResurgence.SST.Utilities;

namespace QResurgence.SST.example
{
    internal class Program
    {
        private static void Main()
        {
            using (IServer server = new AdditionServer())
            {
                server.Register<AdditionCapability>();
                server.Serve();

                IClient client = new AdditionClient();

                client.GetCapability<IAdditionCapability, AdditionArguments, AdditionReturnValue>()
                    .Map(capability =>
                        capability.Invoke(new AdditionArguments(2, 3))
                            .Map(result =>
                                result.Value == 5
                                    ? result
                                    : new Left<IError, AdditionReturnValue>(new CalculationError()))
                    )
                    .Case<IError, AdditionReturnValue, AdditionReturnValue>(rv => Console.WriteLine("Got good answer."))
                    .Fold(_ => Console.WriteLine("Capability request denied."), error => error is RequestDeniedError)
                    .Fold(_ => Console.WriteLine("Capability invocation error occured."),
                        error => error is CapabilityInvocationError)
                    .Fold(_ => Console.WriteLine("Calculation error occured."), error => error is CalculationError)
                    .Fold(_ => Console.WriteLine("Unknown error occured."));
            }
        }
    }

    internal class CalculationError : IError
    {
    }
}