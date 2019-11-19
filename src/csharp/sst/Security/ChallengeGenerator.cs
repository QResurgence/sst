using System;
using System.Linq;
using QResurgence.SST.Messages;

namespace QResurgence.SST.Security
{
    internal static class ChallengeGenerator
    {
        public static Challenge Generate(Guid requesterIdentity)
        {
            var r = new Random(BitConverter.ToInt32(requesterIdentity.ToByteArray().Take(4).ToArray(), 0));
            var left = r.Next(1, 10000);
            var right = r.Next(1, 10000);

            return new Challenge(left, right);
        }
    }
}