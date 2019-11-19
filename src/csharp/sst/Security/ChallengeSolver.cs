using QResurgence.SST.Messages;

namespace QResurgence.SST.Security
{
    internal static class ChallengeSolver
    {
        public static Solution Solve(Challenge challenge) => new Solution(challenge.Left + challenge.Right);
    }
}