namespace QResurgence.SST.example
{
    internal class AdditionArguments
    {
        public AdditionArguments(int left, int right)
        {
            Left = left;
            Right = right;
        }

        public int Left { get; }
        public int Right { get; }
    }
}