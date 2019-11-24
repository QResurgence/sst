using NUnit.Framework;
using QResurgence.SST.Errors;
using QResurgence.SST.Extensions;
using QResurgence.SST.UnitTests.TestImplementation;

namespace QResurgence.SST.UnitTests
{
    public class CommunicationTests
    {
        [Test]
        public void ImplementationCorrectTest()
        {
            using (IServer server = new AdditionServer())
            using (IClient client = new AdditionClient())
            {
                server.Register<AdditionCapability>();
                server.Serve();

                var gotCorrectAnswer = false;
                var gotCalculationError = false;
                var gotOtherError = false;
                var finishedUnsuccessfully = false;

                client.GetCapability<IAdditionCapability, AdditionArguments, AdditionReturnValue>()
                    .Map(capability => capability.Invoke(new AdditionArguments(2, 3)))
                    .Map(result => new AdditionSucceeded(), () => new CalculationError(), result => result.Value == 5)
                    .Case((AdditionSucceeded rv) => gotCorrectAnswer = true)
                    .Fold(_ => gotCalculationError = true, error => error is CalculationError)
                    .Fold(_ => gotOtherError = true, error => error is RequestDeniedError)
                    .Fold(_ => gotOtherError = true, error => error is CapabilityInvocationError)
                    .Fold(_ => gotOtherError = true, error => error is UnsuccessfulNegotiationWithServer)
                    .Fold(_ => gotOtherError = true, error => error is UnexpectedMessageError)
                    .Fold(_ => gotOtherError = true, error => error is UnknownMessageError)
                    .Fold(_ => gotOtherError = true, error => error is UnknownError)
                    .Fold(_ => finishedUnsuccessfully = true);

                Assert.True(gotCorrectAnswer);
                Assert.False(gotCalculationError);
                Assert.False(gotOtherError);
                Assert.False(finishedUnsuccessfully);
            }
        }

        [Test]
        public void ImplementationIncorrectTest()
        {
            using (IServer server = new AdditionServer())
            using (IClient client = new AdditionClient())
            {
                server.Register<AdditionIncorrectCapability>();
                server.Serve();

                var gotCorrectAnswer = false;
                var gotCalculationError = false;
                var gotOtherError = false;
                var finishedUnsuccessfully = false;

                client.GetCapability<IAdditionCapability, AdditionArguments, AdditionReturnValue>()
                    .Map(capability => capability.Invoke(new AdditionArguments(2, 3)))
                    .Map(result => new AdditionSucceeded(), () => new CalculationError(), result => result.Value == 5)
                    .Case((AdditionSucceeded rv) => gotCorrectAnswer = true)
                    .Fold(_ => gotCalculationError = true, error => error is CalculationError)
                    .Fold(_ => gotOtherError = true, error => error is RequestDeniedError)
                    .Fold(_ => gotOtherError = true, error => error is CapabilityInvocationError)
                    .Fold(_ => gotOtherError = true, error => error is UnsuccessfulNegotiationWithServer)
                    .Fold(_ => gotOtherError = true, error => error is UnexpectedMessageError)
                    .Fold(_ => gotOtherError = true, error => error is UnknownMessageError)
                    .Fold(_ => gotOtherError = true, error => error is UnknownError)
                    .Fold(_ => finishedUnsuccessfully = true);

                Assert.False(gotCorrectAnswer);
                Assert.True(gotCalculationError);
                Assert.False(gotOtherError);
                Assert.True(finishedUnsuccessfully);
            }
        }

        [Test]
        public void ImplementationThrowsExceptionTest()
        {
            using (IServer server = new AdditionServer())
            using (IClient client = new AdditionClient())
            {
                server.Register<AdditionThrowsExceptionCapability>();
                server.Serve();

                var gotCorrectAnswer = false;
                var gotCalculationError = false;
                var gotOtherError = false;
                var finishedUnsuccessfully = false;

                client.GetCapability<IAdditionCapability, AdditionArguments, AdditionReturnValue>()
                    .Map(capability => capability.Invoke(new AdditionArguments(2, 3)))
                    .Map(result => new AdditionSucceeded(), () => new CalculationError(), result => result.Value == 5)
                    .Case((AdditionSucceeded rv) => gotCorrectAnswer = true)
                    .Fold(_ => gotCalculationError = true, error => error is CalculationError)
                    .Fold(_ => gotOtherError = true, error => error is RequestDeniedError)
                    .Fold(_ => gotOtherError = true, error => error is CapabilityInvocationError)
                    .Fold(_ => gotOtherError = true, error => error is UnsuccessfulNegotiationWithServer)
                    .Fold(_ => gotOtherError = true, error => error is UnexpectedMessageError)
                    .Fold(_ => gotOtherError = true, error => error is UnknownMessageError)
                    .Fold(_ => gotOtherError = true, error => error is UnknownError)
                    .Fold(_ => finishedUnsuccessfully = true);

                Assert.False(gotCorrectAnswer);
                Assert.False(gotCalculationError);
                Assert.True(gotOtherError);
                Assert.True(finishedUnsuccessfully);
            }
        }
    }
}