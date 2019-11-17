using System;

namespace QResurgence.SST.Utilities
{
    /// <summary>
    ///     Container type which may contain a value of the specified type
    /// </summary>
    /// <typeparam name="T">The type of the value maybe contained</typeparam>
    public class Maybe<T>
    {
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
            Content = content;
        }

        private T Content { get; }

        /// <summary>
        ///     Evaluates the handler if the container contains a value
        /// </summary>
        /// <param name="handler">The handler</param>
        /// <returns>The container</returns>
        public Maybe<T> Just(Action<T> handler)
        {
            if (Content == null) return this;

            handler?.Invoke(Content);
            return this;
        }

        /// <summary>
        ///     Evaluates the handler if the container contains nothing
        /// </summary>
        /// <param name="handler">The handler</param>
        public void Nothing(Action handler)
        {
            if (Content != null) return;

            handler?.Invoke();
        }


        //public static implicit operator T(Maybe<T> maybe)
        //{
        //    if (maybe.Content == null) throw new Exception("Cannot cast nothing to something.");

        //    return maybe.Content;
        //}
    }
}