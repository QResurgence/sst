namespace QResurgence.SST.Utilities
{
    /// <summary>
    ///     Implements the container of the right value
    /// </summary>
    /// <typeparam name="TLeft">The left value type</typeparam>
    /// <typeparam name="TRight">The right value type</typeparam>
    public class Right<TLeft, TRight> : Either<TLeft, TRight>
    {
        /// <inheritdoc />
        public Right(TRight right) : base(right)
        {
        }
    }
}