using System;
using System.Text;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using QResurgence.SST.Errors;
using QResurgence.SST.Messages;
using QResurgence.SST.Security;
using QResurgence.SST.Utilities;

namespace QResurgence.SST.Capability
{
    /// <summary>
    ///     Implements the capability client responsible for communicating with the server providing the capability
    /// </summary>
    /// <typeparam name="TArgument"></typeparam>
    /// <typeparam name="TReturn"></typeparam>
    public class CapabilityClient<TArgument, TReturn>
    {
        private readonly CapabilityInfo _info;
        private readonly SecurityNegotiationClient _negotiator;
        private readonly RequestSocket _socket;

        /// <summary>
        ///     Initializes an instance of the <see cref="CapabilityClient{TArgument,TReturn}" /> class
        /// </summary>
        /// <param name="socket">The socket used for communication</param>
        /// <param name="info">The capability info</param>
        /// <param name="negotiator">The security negotiator</param>
        internal CapabilityClient(RequestSocket socket, CapabilityInfo info, SecurityNegotiationClient negotiator)
        {
            _socket = socket;
            _info = info;
            _negotiator = negotiator;
        }

        /// <summary>
        ///     Invokes the capability and returns the return value
        /// </summary>
        /// <param name="argument">The invocation argument</param>
        /// <returns>The return value</returns>
        public Either<IError, TReturn> Invoke(TArgument argument)
        {
            _socket.SendMultipartMessage(CreateRequest(argument));

            var response = _socket.ReceiveMultipartMessage();
            var messageType = (MessageType) response.Pop().ConvertToInt32();

            switch (messageType)
            {
                case MessageType.CapabilityInvocationResult:
                    return new Right<IError, TReturn>(
                        JsonConvert.DeserializeObject<TReturn>(response.Pop().ConvertToString()));
                case MessageType.Error:
                    var error = JsonConvert.DeserializeObject<ErrorMessage>(response.Pop().ConvertToString());
                    return new Left<IError, TReturn>(ErrorUtilities.GetErrorByErrorCode(error.ErrorCode));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private NetMQMessage CreateRequest(TArgument argument)
        {
            var request = new NetMQMessage();
            request.Append((int) MessageType.InvokeCapability);
            request.Append(_negotiator.Encrypt(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_info))));
            request.Append(_negotiator.Encrypt(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(argument))));

            return request;
        }
    }
}