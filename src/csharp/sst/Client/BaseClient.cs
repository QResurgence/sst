using System;
using System.Net;
using System.Text;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using QResurgence.SST.Capability;
using QResurgence.SST.Errors;
using QResurgence.SST.Extensions;
using QResurgence.SST.Messages;
using QResurgence.SST.Utilities;
using ErrorCode = QResurgence.SST.Messages.ErrorCode;

namespace QResurgence.SST.Client
{
    /// <summary>
    ///     Implements the base client interface
    /// </summary>
    public abstract class BaseClient : IClient
    {
        /// <inheritdoc />
        public Either<IError, CapabilityClient<TArgument, TReturn>> GetCapability<TICapability, TArgument, TReturn>()
            where TICapability : ICapabilityDefinition
        {
            var name = CapabilityUtilities.GetCapabilityName<TICapability>();

            using (var requestSocket = new RequestSocket())
            {
                requestSocket.Connect("tcp://127.0.0.1:9000");

                return GetCapabilityFromServer(requestSocket, name)
                    .Map(info => new CapabilityClient<TArgument, TReturn>(info, IPAddress.Loopback, 9000));
            }
        }

        private Either<IError, CapabilityInfo> GetCapabilityFromServer(RequestSocket socket, string name)
        {
            socket.SendMultipartMessage(CreateGetCapabilityMessage(name));

            var response = socket.ReceiveMultipartMessage();
            var messageType = (MessageType) response.Pop().ConvertToInt32();

            switch (messageType)
            {
                case MessageType.GrantCapability:
                    var info = JsonConvert.DeserializeObject<CapabilityInfo>(
                        Encoding.UTF8.GetString(response.Pop().ToByteArray()));
                    return new Right<IError, CapabilityInfo>(info);
                case MessageType.Error:
                    var error = JsonConvert.DeserializeObject<ErrorMessage>(
                        Encoding.UTF8.GetString(response.Pop().ToByteArray()));
                    return new Left<IError, CapabilityInfo>(GetError(error.ErrorCode));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IError GetError(ErrorCode errorCode)
        {
            switch (errorCode)
            {
                case ErrorCode.RequestDenied:
                    return new RequestDeniedError();
                case ErrorCode.InvocationError:
                    return new CapabilityInvocationError();
                default:
                    throw new ArgumentOutOfRangeException(nameof(errorCode), errorCode, null);
            }
        }

        private static NetMQMessage CreateGetCapabilityMessage(string name)
        {
            var request = new NetMQMessage();
            request.Append((int) MessageType.GetCapability);
            request.Append(SerializeCapabilityInfo(new CapabilityInfo(name)));
            return request;
        }

        private static byte[] SerializeCapabilityInfo(CapabilityInfo info) =>
            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(info));
    }
}