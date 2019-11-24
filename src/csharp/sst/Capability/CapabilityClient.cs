using System;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using QResurgence.SST.Errors;
using QResurgence.SST.Messages;
using QResurgence.SST.Security;
using QResurgence.SST.Utilities;
using ErrorCode = QResurgence.SST.Messages.ErrorCode;

namespace QResurgence.SST.Capability
{
    /// <summary>
    ///     Implements the capability client responsible for communicating with the server providing the capability
    /// </summary>
    /// <typeparam name="TArgument"></typeparam>
    /// <typeparam name="TReturn"></typeparam>
    public class CapabilityClient<TArgument, TReturn>
    {
        private readonly SymetricEncryption _encryptor;
        private readonly CapabilityInfo _info;
        private readonly RequestSocket _socket;

        /// <summary>
        ///     Initializes an instance of the <see cref="CapabilityClient{TArgument,TReturn}" /> class
        /// </summary>
        /// <param name="socket">The socket used for communication</param>
        /// <param name="info">The capability info</param>
        /// <param name="encryptor">The encryption client</param>
        internal CapabilityClient(RequestSocket socket, CapabilityInfo info, SymetricEncryption encryptor)
        {
            _socket = socket;
            _info = info;
            _encryptor = encryptor;
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
                        JsonConvert.DeserializeObject<TReturn>(_encryptor.Decrypt(response.Pop().ToByteArray())));
                case MessageType.Error:
                    var error = JsonConvert.DeserializeObject<ErrorCode>(response.Pop().ConvertToString());
                    return new Left<IError, TReturn>(ErrorUtilities.GetErrorByErrorCode(error));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private NetMQMessage CreateRequest(TArgument argument)
        {
            var request = new NetMQMessage();
            request.Append((int) MessageType.InvokeCapability);
            request.Append(_encryptor.Encrypt(JsonConvert.SerializeObject(_info)));
            request.Append(_encryptor.Encrypt(JsonConvert.SerializeObject(argument)));

            return request;
        }
    }
}