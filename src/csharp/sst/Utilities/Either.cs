namespace QResurgence.SST.Utilities
{
    /// <summary>
    ///     Defines a container type which may hold a value of either <typeparamref name="TLeft" /> or of
    ///     <typeparamref name="TRight" />
    /// </summary>
    /// <typeparam name="TLeft">The type of a value generally considered to be an error</typeparam>
    /// <typeparam name="TRight">The type of a value generally considered to be an allowed value</typeparam>
    public abstract class Either<TLeft, TRight>
    {
        private readonly TLeft _left;
        private readonly TRight _right;

        /// <summary>
        ///     Initializes an instance of the Either container type in case the contained type is a <typeparamref name="TLeft" />
        /// </summary>
        /// <param name="left">The value</param>
        protected Either(TLeft left)
        {
            _left = left;
        }

        /// <summary>
        ///     Initializes an instance of the Either container type in case the contained type is a <typeparamref name="TRight" />
        /// </summary>
        /// <param name="right">The value</param>
        protected Either(TRight right)
        {
            _right = right;
        }

        /// <summary>
        ///     Implements the implicit cast of the container type into its <typeparamref name="TLeft" /> value
        /// </summary>
        /// <param name="either">The container</param>
        /// <returns>The value</returns>
        public static implicit operator TLeft(Either<TLeft, TRight> either) => either._left;

        /// <summary>
        ///     Implements the implicit cast of the container type into its <typeparamref name="TRight" /> value
        /// </summary>
        /// <param name="either">The container</param>
        /// <returns>The value</returns>
        public static implicit operator TRight(Either<TLeft, TRight> either) => either._right;

        internal bool IsLeft() => _left != null;

        internal bool IsRight() => _right != null;
    }
}