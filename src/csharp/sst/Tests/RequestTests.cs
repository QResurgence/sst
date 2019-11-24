using System;
using NetMQ;
using Newtonsoft.Json;
using NUnit.Framework;
using QResurgence.SST.Messages;
using QResurgence.SST.Security;
using ErrorCode = QResurgence.SST.Messages.ErrorCode;

namespace QResurgence.SST.Tests
{
    internal class RequestTests
    {
        [Test]
        public void RequestCreateWithPayloadTest()
        {
            var payload = JsonConvert.SerializeObject(ErrorCode.InvocationError);
            var request = RequestCreator.Create(new NoEncryption(), MessageType.Error, payload);

            Assert.AreEqual(2, request.FrameCount);

            var type = (MessageType) request.Pop().ConvertToInt32();
            Assert.AreEqual(MessageType.Error, type);

            var errorCode = JsonConvert.DeserializeObject<ErrorCode>(request.Pop().ConvertToString());
            Assert.AreEqual(ErrorCode.InvocationError, errorCode);
        }

        [Test]
        public void RequestCreateWithoutPayloadTest()
        {
            var request = RequestCreator.Create(new NoEncryption(), MessageType.Acknowledge);

            Assert.AreEqual(2, request.FrameCount);

            var type = (MessageType) request.Pop().ConvertToInt32();
            Assert.AreEqual(MessageType.Acknowledge, type);

            var payload = request.Pop().ToByteArray();
            Assert.AreEqual(0, payload.Length);
        }

        [Test]
        public void RequestReadWithPayloadTest()
        {
            var requestMessage = new NetMQMessage();

            var requester = Guid.NewGuid();
            requestMessage.Append(requester.ToByteArray());
            requestMessage.AppendEmptyFrame();

            requestMessage.Append((int) MessageType.Error);
            requestMessage.Append(JsonConvert.SerializeObject(ErrorCode.InvocationError));

            var request = RequestReader.Read<ErrorCode>(new NoEncryption(), requestMessage);

            Assert.AreEqual(requester, new Guid(request.From));
            Assert.AreEqual(MessageType.Error, request.Type);
            Assert.AreEqual(ErrorCode.InvocationError, request.Payload);
        }

        /// <summary>
        ///     Test for reading requests without a payload
        /// </summary>
        [Test]
        public void RequestReadWithoutPayloadTest()
        {
            var requestMessage = new NetMQMessage();

            var requester = Guid.NewGuid();
            requestMessage.Append(requester.ToByteArray());
            requestMessage.AppendEmptyFrame();

            requestMessage.Append((int) MessageType.Acknowledge);
            requestMessage.AppendEmptyFrame();

            var request = RequestReader.Read(new NoEncryption(), requestMessage);

            Assert.AreEqual(requester, new Guid(request.From));
            Assert.AreEqual(MessageType.Acknowledge, request.Type);
            Assert.IsNull(request.Payload);
        }
    }
}