using System;
using System.Collections.Generic;
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
        private readonly AsymetricDecryptor _decryptor;
        private readonly Dictionary<Guid, SymetricEncryption> _encryptionKeys;
        private readonly Dictionary<Guid, Solution> _solutions;

        public SecurityNegotiationServer()
        {
            _decryptor = new AsymetricDecryptor();

            _encryptionKeys = new Dictionary<Guid, SymetricEncryption>();
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

        public IEncryptor GetEncryptorFor(Guid requesterIdentity)
        {
            return _encryptionKeys[requesterIdentity];
        }

        public IDecryptor GetDecryptorFor(Guid requesterIdentity)
        {
            return _encryptionKeys[requesterIdentity];
        }

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

            var solutionDecryptedJson = GetDecryptorFor(requesterIdentity).Decrypt(requestContent);

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

            var encryptionKey = _decryptor.Decrypt(requestContent);
            StoreEncryptionKey(requesterIdentity, JsonConvert.DeserializeObject<EncryptionKey>(encryptionKey));
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
            var response = ResponseCreator.Create(new NoEncryption(), requester, MessageType.Acknowledge);

            router.SendMultipartMessage(response);
        }

        private void SendChallenge(byte[] requester, Guid requesterIdentity, RouterSocket router, Challenge challenge)
        {
            var challengeJson = JsonConvert.SerializeObject(challenge);
            var encryptor = GetEncryptorFor(requesterIdentity);

            var response = ResponseCreator.Create(encryptor, requester, MessageType.SendChallenge, challengeJson);

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

                _encryptionKeys.Add(requesterIdentity, new SymetricEncryption(encryptionKey));
            }
        }

        private void SendPublicKey(byte[] requester, RouterSocket router)
        {
            var response = ResponseCreator.Create(new NoEncryption(), requester, MessageType.SendPublicKey,
                JsonConvert.SerializeObject(_decryptor.PublicKey));

            router.SendMultipartMessage(response);
        }
    }
}