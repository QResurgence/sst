using System;

namespace QResurgence.SST.Utilities
{
    /// <summary>
    ///     Container type which may contain a value of the specified type
    /// </summary>
    /// <typeparam name="T">The type of the value maybe contained</typeparam>
    public class Maybe<T>
    {
        private readonly T _content;

        /// <summary>
        ///     Initializes an instance of the <see cref="Maybe{T}" /> class when it contains nothing
        /// </summary>
        protected Maybe()
        {
        }

        /// <summary>
        ///     Initializes an instance of the container which does contain a value of the <typeparamref name="T" /> type
        /// </summary>
        /// <param name="content">The value</param>
        public Maybe(T content)
        {
            _content = content;
        }

        /// <summary>
        ///     Evaluates the handler if the container contains a value
        /// </summary>
        /// <param name="handler">The handler</param>
        /// <returns>The container</returns>
        public Maybe<T> Just(Action<T> handler)
        {
            if (_content == null) return this;

            handler?.Invoke(_content);
            return this;
        }

        /// <summary>
        ///     Evaluates the handler if the container contains nothing
        /// </summary>
        /// <param name="handler">The handler</param>
        public void Nothing(Action handler)
        {
            if (_content != null) return;

            handler?.Invoke();
        }
    }
}