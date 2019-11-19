using System;
using QResurgence.SST.Errors;
using QResurgence.SST.Extensions;

namespace QResurgence.SST.example
{
    internal class Program
    {
        private static void Main()
        {
            using (IServer server = new AdditionServer())
            using (IClient client = new AdditionClient())
            {
                server.Register<AdditionCapability>();
                server.Serve();

                client.GetCapability<IAdditionCapability, AdditionArguments, AdditionReturnValue>()
                    .Map(capability => capability.Invoke(new AdditionArguments(2, 3)))
                    .Map(result => new AdditionSucceeded(), () => new CalculationError(), result => result.Value == 5)
                    .Case((AdditionSucceeded rv) => Console.WriteLine("Got good answer."))
                    .Fold(_ => Console.WriteLine("Calculation error occured."), error => error is CalculationError)
                    .Fold(_ => Console.WriteLine("Capability request denied."), error => error is RequestDeniedError)
                    .Fold(_ => Console.WriteLine("Capability invocation error occured."),
                        error => error is CapabilityInvocationError)
                    .Fold(_ => Console.WriteLine("Security negotiation with server was unsuccessful."),
                        error => error is UnsuccessfulNegotiationWithServer)
                    .Fold(_ => Console.WriteLine("Unexpected message was sent."),
                        error => error is UnexpectedMessageError)
                    .Fold(_ => Console.WriteLine("Unknown message was sent."), error => error is UnknownMessageError)
                    .Fold(_ => Console.WriteLine("Unknown error."), error => error is UnknownError)
                    .Fold(_ => Console.WriteLine("Finished unsuccessfully."));
            }
        }
    }

    internal class CalculationError : IError
    {
    }
}