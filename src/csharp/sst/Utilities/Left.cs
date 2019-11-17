namespace QResurgence.SST.Utilities
{
    /// <summary>
    ///     Implements the container of the left value
    /// </summary>
    /// <typeparam name="TLeft">The left value type</typeparam>
    /// <typeparam name="TRight">The right value type</typeparam>
    public class Left<TLeft, TRight> : Either<TLeft, TRight>
    {
        /// <inheritdoc />
        public Left(TLeft left) : base(left)
        {
        }
    }
}