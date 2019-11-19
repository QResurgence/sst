using System;
using QResurgence.SST.Utilities;

namespace QResurgence.SST.Extensions
{
    /// <summary>
    ///     Implements the Either container type extensions
    /// </summary>
    public static class EitherExtensions
    {
        /// <summary>
        ///     Implements the mapping of the <typeparamref name="TRight" /> value into the <typeparamref name="TNewRight" /> value
        ///     or returns the container of the left value
        /// </summary>
        /// <param name="either">The container</param>
        /// <param name="map">
        ///     The function that maps <typeparamref name="TRight" /> value onto the
        ///     <typeparamref name="TNewRight" /> value
        /// </param>
        /// <typeparam name="TLeft">The left value type</typeparam>
        /// <typeparam name="TRight">The right value type to be mapped</typeparam>
        /// <typeparam name="TNewRight">The right value type to be mapped to</typeparam>
        /// <returns>The resulting container type of the <typeparamref name="TLeft" /> or <typeparamref name="TNewRight" /> value</returns>
        public static Either<TLeft, TNewRight> Map<TLeft, TRight, TNewRight>(this Either<TLeft, TRight> either,
            Func<TRight, TNewRight> map)
        {
            if (!either.IsRight()) return new Left<TLeft, TNewRight>(either);

            return new Right<TLeft, TNewRight>(map(either));
        }

        /// <summary>
        ///     Implements the mapping of the <typeparamref name="TRight" /> value into the <typeparamref name="TNewRight" /> value
        ///     when mapping can result in a either <typeparamref name="TNewRight" /> value or <typeparamref name="TLeft" /> value
        /// </summary>
        /// <param name="either">The container</param>
        /// <param name="map">
        ///     The function that maps <typeparamref name="TRight" /> value onto the
        ///     <typeparamref name="TNewRight" /> value or the <typeparamref name="TLeft" /> value
        /// </param>
        /// <typeparam name="TLeft">The left value type</typeparam>
        /// <typeparam name="TRight">The right value type to be mapped</typeparam>
        /// <typeparam name="TNewRight">The right value type to be mapped to</typeparam>
        /// <returns>The resulting container type of the <typeparamref name="TLeft" /> or <typeparamref name="TNewRight" /> value</returns>
        public static Either<TLeft, TNewRight> Map<TLeft, TRight, TNewRight>(
            this Either<TLeft, TRight> either,
            Func<TRight, Either<TLeft, TNewRight>> map) =>
            !either.IsRight() ? new Left<TLeft, TNewRight>(either) : map(either);

        /// <summary>
        ///     Implements conditional mapping of the <typeparamref name="TRight" /> value into the
        ///     <typeparamref name="TNewRight" /> value
        ///     when a predicate is satisfied
        /// </summary>
        /// <param name="either">The container</param>
        /// <param name="predicateSatisfiedHandler">Handler evaluated if the predicate is satisfied</param>
        /// <param name="predicateNotSatisfiedHandler">Handler evaluated if the predicate is not satisfied</param>
        /// <param name="predicate">The predicate</param>
        /// <typeparam name="TLeft">The left value type</typeparam>
        /// <typeparam name="TRight">The right value type to be mapped</typeparam>
        /// <typeparam name="TNewRight">The right value type to be mapped to</typeparam>
        /// <returns>The resulting container type of the <typeparamref name="TLeft" /> or <typeparamref name="TNewRight" /> value</returns>
        public static Either<TLeft, TNewRight> Map<TLeft, TRight, TNewRight>(
            this Either<TLeft, TRight> either,
            Func<TRight, TNewRight> predicateSatisfiedHandler,
            Func<TLeft> predicateNotSatisfiedHandler,
            Func<TRight, bool> predicate) =>
            !either.IsRight() || !predicate(either)
                ? (Either<TLeft, TNewRight>) new Left<TLeft, TNewRight>(predicateNotSatisfiedHandler())
                : new Right<TLeft, TNewRight>(predicateSatisfiedHandler(either));

        /// <summary>
        ///     Evaluates the <paramref name="action" /> over the contained right value if the contained value is of
        ///     <typeparamref name="TCase" /> type
        ///     or returns the container of left value
        /// </summary>
        /// <param name="either">The container</param>
        /// <param name="action">The action to be performed over the contained value if conditions are satisfied</param>
        /// <typeparam name="TLeft">The left value type</typeparam>
        /// <typeparam name="TRight">The right value type</typeparam>
        /// <typeparam name="TCase">The type which defines the condition</typeparam>
        /// <returns>The container</returns>
        public static Either<TLeft, TRight> Case<TLeft, TRight, TCase>(this Either<TLeft, TRight> either,
            Action<TCase> action) where TCase : class
        {
            if (!either.IsRight()) return new Left<TLeft, TRight>(either);

            if ((TRight) either is TCase v) action(v);

            return either;
        }

        /// <summary>
        ///     Evaluates the <paramref name="action" /> over the contained left value if the contained value satisfied the
        ///     <paramref name="predicate" />
        /// </summary>
        /// <param name="either">The container</param>
        /// <param name="action">The action to be evaluated if the predicate is satisfied</param>
        /// <param name="predicate">The predicate</param>
        /// <typeparam name="TLeft">The left value type</typeparam>
        /// <typeparam name="TRight">The right value type</typeparam>
        /// <returns>The container</returns>
        public static Either<TLeft, TRight> Fold<TLeft, TRight>(this Either<TLeft, TRight> either, Action<TLeft> action,
            Func<TLeft, bool> predicate) where TLeft : class
        {
            if (!either.IsLeft()) return new Right<TLeft, TRight>(either);

            if (predicate(either)) action.Invoke(either);

            return either;
        }

        /// <summary>
        ///     Evaluates the action if the contained value is a left value. Used as the final fold
        /// </summary>
        /// <param name="either">The container</param>
        /// <param name="action">The action</param>
        /// <typeparam name="TLeft">The left value type</typeparam>
        /// <typeparam name="TRight">The right value type</typeparam>
        public static void Fold<TLeft, TRight>(this Either<TLeft, TRight> either, Action<TLeft> action)
            where TLeft : class
        {
            if (!either.IsLeft()) return;

            action.Invoke(either);
        }
    }
}