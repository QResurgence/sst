namespace QResurgence.SST.Messages
{
    internal class Challenge
    {
        public Challenge(int left, int right)
        {
            Left = left;
            Right = right;
        }

        public int Left { get; }

        public int Right { get; }
    }
}