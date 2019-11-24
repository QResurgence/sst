using System;
using System.Text;
using NetMQ;
using Newtonsoft.Json;
using NUnit.Framework;
using QResurgence.SST.Messages;
using QResurgence.SST.Security;
using ErrorCode = QResurgence.SST.Messages.ErrorCode;

namespace QResurgence.SST.Tests
{
    internal class ResponseTests
    {
        [Test]
        public void ResponseCreateWithPayloadTest()
        {
            var requester = Guid.NewGuid();
            var payload = JsonConvert.SerializeObject(ErrorCode.InvocationError);

            var response = ResponseCreator.Create(new NoEncryption(), requester.ToByteArray(), MessageType.Error, payload);
            
            Assert.AreEqual(4, response.FrameCount);

            var to = new Guid(response.Pop().ToByteArray());
            Assert.AreEqual(requester, to);
            
            Assert.True(response.Pop().IsEmpty);

            var type = (MessageType) response.Pop().ConvertToInt32();
            Assert.AreEqual(MessageType.Error, type);
            
            var decodedPayload = JsonConvert.DeserializeObject<ErrorCode>(response.Pop().ConvertToString());
            Assert.AreEqual(ErrorCode.InvocationError, decodedPayload);
        }
        
        [Test]
        public void ResponseCreateWithoutPayloadTest()
        {
            var requester = Guid.NewGuid();

            var response = ResponseCreator.Create(new NoEncryption(), requester.ToByteArray(), MessageType.Acknowledge);
            
            Assert.AreEqual(4, response.FrameCount);

            var to = new Guid(response.Pop().ToByteArray());
            Assert.AreEqual(requester, to);
            
            Assert.True(response.Pop().IsEmpty);

            var type = (MessageType) response.Pop().ConvertToInt32();
            Assert.AreEqual(MessageType.Acknowledge, type);

            Assert.True(response.Pop().IsEmpty);
        }

        [Test]
        public void ResponseReadWithPayloadTest()
        {
            var responseMessage = new NetMQMessage();
            responseMessage.Append((int)MessageType.Error);
            responseMessage.Append(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ErrorCode.InvocationError)));

            var response = ResponseReader.Read<ErrorCode>(new NoEncryption(), responseMessage);
            
            Assert.AreEqual(MessageType.Error, response.Type);
            Assert.AreEqual(ErrorCode.InvocationError, response.Payload);
        }
        
        [Test]
        public void ResponseReadWithoutPayloadTest()
        {
            var responseMessage = new NetMQMessage();
            responseMessage.Append((int)MessageType.Acknowledge);
            responseMessage.AppendEmptyFrame();

            var response = ResponseReader.Read(new NoEncryption(), responseMessage);
            
            Assert.AreEqual(MessageType.Acknowledge, response.Type);
            Assert.IsNull(response.Payload);
        }
    }
}