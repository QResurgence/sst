using System;
using System.Text;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using QResurgence.SST.Capability;
using QResurgence.SST.Errors;
using QResurgence.SST.Extensions;
using QResurgence.SST.Messages;
using QResurgence.SST.Security;
using QResurgence.SST.Utilities;

namespace QResurgence.SST.Client
{
    /// <summary>
    ///     Implements the base client interface
    /// </summary>
    public abstract class BaseClient : IClient
    {
        private readonly SecurityNegotiationClient _negotiator;
        private readonly RequestSocket _requestSocket;
        private readonly SymetricEncryption _encryptor;

        /// <summary>
        ///     Initializes an instance of the <see cref="BaseClient" /> class
        /// </summary>
        protected BaseClient()
        {
            _encryptor = new SymetricEncryption();
            _negotiator = new SecurityNegotiationClient(_encryptor);
            _requestSocket = new RequestSocket();
            _requestSocket.Options.Identity = Guid.NewGuid().ToByteArray();
            _requestSocket.Connect("tcp://127.0.0.1:9000");
        }

        /// <inheritdoc />
        public Either<IError, CapabilityClient<TArgument, TReturn>> GetCapability<TICapability, TArgument, TReturn>()
            where TICapability : ICapabilityDefinition
        {
            var name = CapabilityUtilities.GetCapabilityName<TICapability>();

            return !_negotiator.Negotiate(_requestSocket)
                ? new Left<IError, CapabilityClient<TArgument, TReturn>>(new UnsuccessfulNegotiationWithServer())
                : GetCapabilityFromServer(name)
                    .Map(info => new CapabilityClient<TArgument, TReturn>(_requestSocket, info, _negotiator, _encryptor));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _requestSocket?.Dispose();
        }

        private Either<IError, CapabilityInfo> GetCapabilityFromServer(string name)
        {
            var message = RequestCreator.Create(_encryptor, MessageType.GetCapability, JsonConvert.SerializeObject(new CapabilityInfo(name)));
            _requestSocket.SendMultipartMessage(message);

            var response = _requestSocket.ReceiveMultipartMessage();
            var messageType = (MessageType) response.Pop().ConvertToInt32();

            switch (messageType)
            {
                case MessageType.GrantCapability:
                    var infoJson = _encryptor.Decrypt(response.Pop().ToByteArray());
                    var info = JsonConvert.DeserializeObject<CapabilityInfo>(infoJson);
                    return new Right<IError, CapabilityInfo>(info);
                case MessageType.Error:
                    var errorContent = response.Pop().ToByteArray();
                    return new Left<IError, CapabilityInfo>(ErrorMessageReceiver.GetError(errorContent));
                default:
                    return new Left<IError, CapabilityInfo>(new UnexpectedMessageError());
            }
        }
    }
}