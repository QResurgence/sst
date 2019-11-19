using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using QResurgence.SST.Messages;
using QResurgence.SST.Server;
using ErrorCode = QResurgence.SST.Messages.ErrorCode;

namespace QResurgence.SST.Security
{
    internal class SecurityNegotiationServer
    {
        private readonly Dictionary<Guid, ChaCha20> _encryptionKeys;
        private readonly string _publicKey;
        private readonly RSACryptoServiceProvider _rsaProvider;
        private readonly Dictionary<Guid, Solution> _solutions;

        public SecurityNegotiationServer()
        {
            _rsaProvider = new RSACryptoServiceProvider();
            _publicKey = _rsaProvider.ToXmlString(false);
            var privateKey = _rsaProvider.ToXmlString(true);

            _rsaProvider.FromXmlString(privateKey);

            _encryptionKeys = new Dictionary<Guid, ChaCha20>();
            _solutions = new Dictionary<Guid, Solution>();
        }

        public void Negotiate(byte[] requester, Guid requesterIdentity, MessageType requestType, byte[] requestContent,
            RouterSocket router)
        {
            try
            {
                HandleMessage(requester, requestType, requestContent, router, requesterIdentity);
            }
            catch (Exception)
            {
                // TODO [2019-00-19] (qresurgence): Add logging
            }
        }

        public byte[] Encrypt(Guid requesterIdentity, string jsonData) =>
            _encryptionKeys[requesterIdentity].EncryptString(jsonData);

        public byte[] Decrypt(Guid requesterIdentity, byte[] data) =>
            _encryptionKeys[requesterIdentity].DecryptBytes(data);

        private void HandleMessage(byte[] requester, MessageType requestType, byte[] requestContent,
            RouterSocket router, Guid requesterIdentity)
        {
            switch (requestType)
            {
                case MessageType.RequestPublicKey:
                    SendPublicKey(requester, router);
                    break;
                case MessageType.SendEncryptionKey:
                    HandleSendEncryptionKey(requester, requestContent, router, requesterIdentity);
                    break;
                case MessageType.ChallengeResponse:
                    HandleChallengeResponse(requester, requestContent, router, requesterIdentity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleChallengeResponse(byte[] requester, byte[] requestContent, RouterSocket router,
            Guid requesterIdentity)
        {
            lock (_solutions)
            {
                if (!_solutions.ContainsKey(requesterIdentity))
                    ErrorMessageSender.SendError(requester, router, ErrorCode.ChallengeFailed);
            }

            var solutionDecryptedJson = Encoding.UTF8.GetString(Decrypt(requesterIdentity, requestContent));

            if (ChallengeResponseIsCorrect(requesterIdentity,
                JsonConvert.DeserializeObject<Solution>(solutionDecryptedJson)))
            {
                RemoveSolutionForRequester(requesterIdentity);
                SendAcknowledge(requester, router);
                return;
            }

            RemoveSolutionForRequester(requesterIdentity);
            ErrorMessageSender.SendError(requester, router, ErrorCode.ChallengeFailed);
        }

        private void HandleSendEncryptionKey(byte[] requester, byte[] requestContent, RouterSocket router,
            Guid requesterIdentity)
        {
            if (_encryptionKeys.ContainsKey(requesterIdentity))
                ErrorMessageSender.SendError(requester, router, ErrorCode.EncryptionKeyAlreadySent);

            var encryptionKey = DecryptWithPrivateKey(requestContent);
            StoreEncryptionKey(requesterIdentity, encryptionKey);
            var challenge = ChallengeGenerator.Generate(requesterIdentity);
            var solution = ChallengeSolver.Solve(challenge);
            SaveSolutionForRequester(requesterIdentity, solution);
            SendChallenge(requester, requesterIdentity, router, challenge);
        }

        private void RemoveSolutionForRequester(Guid requesterIdentity)
        {
            lock (_solutions)
            {
                _solutions.Remove(requesterIdentity);
            }
        }

        private static void SendAcknowledge(byte[] requester, RouterSocket router)
        {
            var response = new NetMQMessage();
            response.Append(requester);
            response.AppendEmptyFrame();
            response.Append((int) MessageType.Acknowledge);
            response.AppendEmptyFrame();

            router.SendMultipartMessage(response);
        }

        private void SendChallenge(byte[] requester, Guid requesterIdentity, RouterSocket router, Challenge challenge)
        {
            var challengeJson = JsonConvert.SerializeObject(challenge);
            var encryptedChallenge = Encrypt(requesterIdentity, challengeJson);

            var response = new NetMQMessage();
            response.Append(requester);
            response.AppendEmptyFrame();
            response.Append((int) MessageType.SendChallenge);
            response.Append(encryptedChallenge);

            router.SendMultipartMessage(response);
        }

        private void SaveSolutionForRequester(Guid requesterIdentity, Solution solution)
        {
            lock (_solutions)
            {
                _solutions.Add(requesterIdentity, solution);
            }
        }

        private bool ChallengeResponseIsCorrect(Guid requesterIdentity, Solution solution)
        {
            lock (_solutions)
            {
                return _solutions[requesterIdentity].Value == solution.Value;
            }
        }

        private void StoreEncryptionKey(Guid requesterIdentity, EncryptionKey encryptionKey)
        {
            lock (_encryptionKeys)
            {
                if (_encryptionKeys.ContainsKey(requesterIdentity)) _encryptionKeys.Remove(requesterIdentity);

                _encryptionKeys.Add(requesterIdentity, new ChaCha20(encryptionKey.Key, encryptionKey.Nonce, 1));
            }
        }

        private EncryptionKey DecryptWithPrivateKey(byte[] requestContent) =>
            JsonConvert.DeserializeObject<EncryptionKey>(
                Encoding.UTF8.GetString(_rsaProvider.Decrypt(requestContent, false)));

        private void SendPublicKey(byte[] requester, RouterSocket router)
        {
            var response = new NetMQMessage();
            response.Append(requester);
            response.AppendEmptyFrame();
            response.Append((int) MessageType.SendPublicKey);
            response.Append(_publicKey);

            router.SendMultipartMessage(response);
        }
    }
}