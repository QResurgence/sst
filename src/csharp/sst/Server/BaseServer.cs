using System;
using System.Text;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using QResurgence.SST.Capability;
using QResurgence.SST.Exceptions;
using QResurgence.SST.Messages;
using QResurgence.SST.Utilities;
using ErrorCode = QResurgence.SST.Messages.ErrorCode;

namespace QResurgence.SST.Server
{
    /// <summary>
    ///     Implements the base server
    /// </summary>
    public abstract class BaseServer : IServer
    {
        private readonly NetMQPoller _poller;
        private readonly CapabilityRegistry _registry;
        private readonly RouterSocket _router;

        /// <summary>
        ///     Initializes an instance of the <see cref="BaseServer" /> class
        /// </summary>
        protected BaseServer()
        {
            _registry = new CapabilityRegistry();

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
            RemoveNullFrame(request);
            var requestType = (MessageType) request.Pop().ConvertToInt32();
            var info = DeserializeCapabilityInfo(request.Pop().ToByteArray());

            switch (requestType)
            {
                case MessageType.GetCapability:
                    HandleGetCapability(requester, info);
                    break;
                case MessageType.InvokeCapability:
                    var requestContent = request.Pop().ToByteArray();
                    HandleInvokeCapability(requester, info, requestContent);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void RemoveNullFrame(NetMQMessage request)
        {
            request.Pop();
        }

        private void HandleInvokeCapability(byte[] requester, CapabilityInfo info, byte[] requestContent)
        {
            _registry.Get(info.Name)
                .Just(capability => { InvokeCapability(requester, capability, requestContent); })
                .Nothing(() => { SendError(requester, ErrorCode.RequestDenied); });
        }

        private void InvokeCapability(byte[] destination, ICapability capability, byte[] arguments)
        {
            try
            {
                var response = new NetMQMessage();
                response.Append(destination);
                response.AppendEmptyFrame();
                response.Append((int) MessageType.CapabilityInvocationResult);
                response.Append(capability.Invoke(arguments));

                _router.SendMultipartMessage(response);
            }
            catch (Exception)
            {
                SendError(destination, ErrorCode.InvocationError);
            }
        }

        private void HandleGetCapability(byte[] requester, CapabilityInfo info)
        {
            _registry.Get(info.Name)
                .Just(capability => { GrantCapability(requester, info); })
                .Nothing(() => { SendError(requester, ErrorCode.RequestDenied); });
        }

        private void GrantCapability(byte[] destination, CapabilityInfo info)
        {
            var response = new NetMQMessage();
            response.Append(destination);
            response.AppendEmptyFrame();
            response.Append((int) MessageType.GrantCapability);
            response.Append(JsonConvert.SerializeObject(info));

            _router.SendMultipartMessage(response);
        }

        private void SendError(byte[] destination, ErrorCode errorCode)
        {
            var response = new NetMQMessage();
            response.Append(destination);
            response.AppendEmptyFrame();
            response.Append((int) MessageType.Error);
            response.Append(JsonConvert.SerializeObject(new ErrorMessage(errorCode)));

            _router.SendMultipartMessage(response);
        }

        private static CapabilityInfo DeserializeCapabilityInfo(byte[] requestContent) =>
            JsonConvert.DeserializeObject<CapabilityInfo>(Encoding.UTF8.GetString(requestContent));
    }
}