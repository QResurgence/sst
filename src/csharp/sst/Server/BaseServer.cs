using System;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using QResurgence.SST.Capability;
using QResurgence.SST.Exceptions;
using QResurgence.SST.Messages;
using QResurgence.SST.Security;
using QResurgence.SST.Utilities;
using ErrorCode = QResurgence.SST.Messages.ErrorCode;

namespace QResurgence.SST.Server
{
    /// <summary>
    ///     Implements the base server
    /// </summary>
    public abstract class BaseServer : IServer
    {
        private readonly SecurityNegotiationServer _negotiator;
        private readonly NetMQPoller _poller;
        private readonly CapabilityRegistry _registry;
        private readonly RouterSocket _router;

        /// <summary>
        ///     Initializes an instance of the <see cref="BaseServer" /> class
        /// </summary>
        protected BaseServer()
        {
            _registry = new CapabilityRegistry();

            _negotiator = new SecurityNegotiationServer();

            _router = new RouterSocket();
            _router.Bind("tcp://*:9000");
            _router.ReceiveReady += HandleRequest;

            _poller = new NetMQPoller {_router};
        }

        /// <inheritdoc />
        public void Serve()
        {
            if (_poller == null) throw new Exception("Woah there!");

            if (_poller.IsRunning) throw new ServerAlreadyRunningException();

            _poller.RunAsync();
        }

        /// <inheritdoc />
        public void Register<TCapability>() where TCapability : ICapability, new()
        {
            var name = CapabilityUtilities.GetCapabilityName<TCapability>();

            lock (_registry)
            {
                if (_registry.Contains(name)) throw new CapabilityAlreadyRegisteredException();

                _registry.Add(name, new TCapability());
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _router.Unbind("tcp://*:9000");
            _router.ReceiveReady -= HandleRequest;
            _poller.RemoveAndDispose(_router);
            _poller.Stop();
        }

        private void HandleRequest(object sender, NetMQSocketEventArgs e)
        {
            var request = e.Socket.ReceiveMultipartMessage();
            var requester = request.Pop().ToByteArray();
            var requesterIdentity = new Guid(requester);
            RemoveNullFrame(request);
            var requestType = (MessageType) request.Pop().ConvertToInt32();


            switch (requestType)
            {
                case MessageType.RequestPublicKey:
                case MessageType.SendEncryptionKey:
                case MessageType.ChallengeResponse:
                {
                    var requestContent = request.Pop().ToByteArray();
                    _negotiator.Negotiate(requester, requesterIdentity, requestType, requestContent, _router);
                }
                    break;
                case MessageType.GetCapability:
                {
                    var decryptedInfoJson = _negotiator.GetDecryptorFor(requesterIdentity)
                        .Decrypt(request.Pop().ToByteArray());
                    var info = DeserializeCapabilityInfo(decryptedInfoJson);
                    HandleGetCapability(requester, requesterIdentity, info);
                }
                    break;
                case MessageType.InvokeCapability:
                {
                    var decryptedInfoJson = _negotiator.GetDecryptorFor(requesterIdentity)
                        .Decrypt(request.Pop().ToByteArray());
                    var info = DeserializeCapabilityInfo(decryptedInfoJson);
                    var requestContent = _negotiator.GetDecryptorFor(requesterIdentity)
                        .Decrypt(request.Pop().ToByteArray());
                    HandleInvokeCapability(requester, requesterIdentity, info, requestContent);
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void RemoveNullFrame(NetMQMessage request)
        {
            request.Pop();
        }

        private void HandleInvokeCapability(byte[] requester, Guid requesterIdentity, CapabilityInfo info,
            string requestContentJson)
        {
            _registry.Get(info.Name)
                .Just(capability => { InvokeCapability(requester, requesterIdentity, capability, requestContentJson); })
                .Nothing(() => { ErrorMessageSender.SendError(requester, _router, ErrorCode.RequestDenied); });
        }

        private void InvokeCapability(byte[] destination, Guid requesterIdentity, ICapability capability,
            string arguments)
        {
            try
            {
                var encryptor = _negotiator.GetEncryptorFor(requesterIdentity);
                var response = ResponseCreator.Create(encryptor, destination, MessageType.CapabilityInvocationResult,
                    capability.Invoke(arguments));

                _router.SendMultipartMessage(response);
            }
            catch (Exception)
            {
                ErrorMessageSender.SendError(destination, _router, ErrorCode.InvocationError);
            }
        }

        private void HandleGetCapability(byte[] requester, Guid requesterIdentity, CapabilityInfo info)
        {
            _registry.Get(info.Name)
                .Just(capability => { GrantCapability(requesterIdentity, requester, info); })
                .Nothing(() => { ErrorMessageSender.SendError(requester, _router, ErrorCode.RequestDenied); });
        }

        private void GrantCapability(Guid requesterIdentity, byte[] destination, CapabilityInfo info)
        {
            var infoJson = JsonConvert.SerializeObject(info);
            var encryptor = _negotiator.GetEncryptorFor(requesterIdentity);

            var response = ResponseCreator.Create(encryptor, destination, MessageType.GrantCapability, infoJson);

            _router.SendMultipartMessage(response);
        }

        private static CapabilityInfo DeserializeCapabilityInfo(string infoJson)
        {
            return JsonConvert.DeserializeObject<CapabilityInfo>(infoJson);
        }
    }
}