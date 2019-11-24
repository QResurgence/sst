using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using QResurgence.SST.Extensions;
using QResurgence.SST.Messages;
using QResurgence.SST.Utilities;
using ErrorCode = NetMQ.ErrorCode;

namespace QResurgence.SST.Security
{
    internal class SecurityNegotiationClient
    {
        private readonly SymetricEncryption _encryptor;

        public SecurityNegotiationClient(SymetricEncryption encryptor)
        {
            _encryptor = encryptor;
        }

        private byte[] GenerateNonce() => Guid.NewGuid().ToByteArray().Take(12).ToArray();

        private static byte[] GenerateEncryptionKey() =>
            Guid.NewGuid().ToByteArray()
                .Aggregate(Guid.NewGuid().ToByteArray(), (current, b) => current.Append(b).ToArray());

        public bool Negotiate(RequestSocket requestSocket)
        {
            var success = false;
            RequestPublicKey(requestSocket)
                .Just(publicKey =>
                {
                    SendEncryptionKey(requestSocket, publicKey);

                    GetChallenge(requestSocket)
                        .Just(challenge =>
                        {
                            var solution = ChallengeSolver.Solve(challenge);
                            SendChallengeResponse(requestSocket, solution);

                            success = GetAcknowledge(requestSocket);
                        });
                });

            return success;
        }

        private static bool GetAcknowledge(RequestSocket requestSocket)
        {
            var response = requestSocket.ReceiveMultipartMessage();
            var messageType = (MessageType) response.Pop().ConvertToInt32();

            return messageType == MessageType.Acknowledge;
        }

        private void SendChallengeResponse(RequestSocket requestSocket, Solution solution)
        {
            var encryptedSolution = _encryptor.Encrypt(JsonConvert.SerializeObject(solution));

            var solutionMessage = new NetMQMessage();
            solutionMessage.Append((int) MessageType.ChallengeResponse);
            solutionMessage.Append(encryptedSolution);

            requestSocket.SendMultipartMessage(solutionMessage);
        }

        private Maybe<Challenge> GetChallenge(RequestSocket requestSocket)
        {
            var challengeMessage = requestSocket.ReceiveMultipartMessage();

            var messageType = (MessageType) challengeMessage.Pop().ConvertToInt32();

            switch (messageType)
            {
                case MessageType.SendChallenge:
                    var encryptedChallenge = challengeMessage.Pop().ToByteArray();
                    return new Maybe<Challenge>(
                        JsonConvert.DeserializeObject<Challenge>(_encryptor.Decrypt(encryptedChallenge)));
                default:
                    return new Nothing<Challenge>();
            }
        }

        private void SendEncryptionKey(RequestSocket requestSocket, string publicKey)
        {
            var encryptionClient = new AsymetricEncryptor(publicKey);
            var encryptionKeyJson = JsonConvert.SerializeObject(_encryptor.EncryptionKey);
            var request = RequestCreator.Create(encryptionClient, MessageType.SendEncryptionKey, encryptionKeyJson);

            requestSocket.SendMultipartMessage(request);
        }

        private static Maybe<string> RequestPublicKey(RequestSocket requestSocket)
        {
            var request = RequestCreator.Create(new NoEncryption(), MessageType.RequestPublicKey);

            requestSocket.SendMultipartMessage(request);

            var responseMessage = requestSocket.ReceiveMultipartMessage();

            Maybe<string> response = null;
            ResponseReader.Read<string>(new NoEncryption(), new NoEncryption(), responseMessage)
                .Case((Response<string> publicKay) => response = new Maybe<string>(publicKay.Payload))
                .Fold(_ => response = new Nothing<string>());

            return response;
        }
    }
}