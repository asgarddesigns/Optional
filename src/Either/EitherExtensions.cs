using System;

namespace Optional
{
    public static class EitherExtensions
    {
        /// <summary>
        /// Wraps an existing value in an Either&lt;T, TException&gt; instance.
        /// </summary>
        /// <param name="value">The value to be wrapped.</param>
        /// <returns>An optional containing the specified value.</returns>
        public static Either<TLeft, TRight> Right<TLeft, TRight>(this TRight value) =>
            Either.Right<TLeft, TRight>(value);

        /// <summary>
        /// Creates an empty Either&lt;T, TException&gt; instance, 
        /// with a specified exceptional value.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="exception">The exceptional value.</param>
        /// <returns>An empty optional.</returns>
        public static Either<TLeft, TRight> Left<TLeft, TRight>(this TRight value, TLeft exception) =>
            Either.Left<TLeft, TRight>(exception);

        /// <summary>
        /// Creates an Either&lt;T&gt; instance from a specified value. 
        /// If the value does not satisfy the given predicate, 
        /// an empty optional is returned.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>An optional containing the specified value.</returns>
        public static Either<TLeft, TRight> RightWhen<TLeft, TRight>(this TRight value, Func<TRight, bool> predicate,
            TLeft leftDefault)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return predicate(value) ? Either.Right<TLeft, TRight>(value) : Either.Left<TLeft, TRight>(leftDefault);
        }

        /// <summary>
        /// Creates an Either&lt;T&gt; instance from a specified value. 
        /// If the value does not satisfy the given predicate, 
        /// an empty optional is returned, with a specified exceptional value.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="exceptionFactory">A factory function to create an exceptional value.</param>
        /// <returns>An optional containing the specified value.</returns>
        public static Either<TLeft, TRight> RightWhen<TLeft, TRight>(this TRight value, Func<TRight, bool> predicate,
            Func<TLeft> leftFactory)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return predicate(value) ? Either.Right<TLeft, TRight>(value) : Either.Left<TLeft, TRight>(leftFactory());
        }

        /// <summary>
        /// Creates an Either&lt;T&gt; instance from a specified value. 
        /// If the value satisfies the given predicate, 
        /// an empty optional is returned, with a specified exceptional value.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="exception">The exceptional value.</param>
        /// <returns>An optional containing the specified value.</returns>
        public static Either<TLeft, TRight> LeftWhen<TLeft, TRight>(this TRight value, Func<TRight, bool> predicate,
            TLeft exception)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return value.RightWhen(val => !predicate(val), exception);
        }

        /// <summary>
        /// Creates an Either&lt;T&gt; instance from a specified value. 
        /// If the value does satisfy the given predicate, 
        /// an empty optional is returned, with a specified exceptional value.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="exceptionFactory">A factory function to create an exceptional value.</param>
        /// <returns>An optional containing the specified value.</returns>
        public static Either<TLeft, TRight> LeftWhen<TLeft, TRight>(this TRight value, Func<TRight, bool> predicate,
            Func<TLeft> exceptionFactory)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (exceptionFactory == null) throw new ArgumentNullException(nameof(exceptionFactory));
            return value.RightWhen(val => !predicate(val), exceptionFactory);
        }

        /// <summary>
        /// Creates an Either&lt;T&gt; instance from a specified value. 
        /// If the value is null, an empty optional is returned, 
        /// with a specified exceptional value.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="exception">The exceptional value.</param>
        /// <returns>An optional containing the specified value.</returns>
        public static Either<TLeft, TRight> RightNotNull<TLeft, TRight>(this TRight value, TLeft exception) =>
            value.RightWhen(val => val != null, exception);

        /// <summary>
        /// Creates an Either&lt;T&gt; instance from a specified value. 
        /// If the value is null, an empty optional is returned, 
        /// with a specified exceptional value.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="exceptionFactory">A factory function to create an exceptional value.</param>
        /// <returns>An optional containing the specified value.</returns>
        public static Either<TLeft, TRight> RightNotNull<TLeft, TRight>(this TRight value, Func<TLeft> exceptionFactory)
        {
            if (exceptionFactory == null) throw new ArgumentNullException(nameof(exceptionFactory));
            return value.RightWhen(val => val != null, exceptionFactory);
        }

        /// <summary>
        /// Converts a Nullable&lt;T&gt; to an Either&lt;T, TException&gt; instance, 
        /// with a specified exceptional value.
        /// </summary>
        /// <param name="value">The Nullable&lt;T&gt; instance.</param>
        /// <param name="exception">The exceptional value.</param>
        /// <returns>The Either&lt;T, TException&gt; instance.</returns>
        public static Either<TLeft, TRight> ToEither<TLeft, TRight>(this TRight? value, TLeft exception)
            where TRight : struct =>
            value.HasValue ? Either.Right<TLeft, TRight>(value.Value) : Either.Left<TLeft, TRight>(exception);

        /// <summary>
        /// Converts a Nullable&lt;T&gt; to an Either&lt;T, TException&gt; instance, 
        /// with a specified exceptional value.
        /// </summary>
        /// <param name="value">The Nullable&lt;T&gt; instance.</param>
        /// <param name="exceptionFactory">A factory function to create an exceptional value.</param>
        /// <returns>The Either&lt;T, TException&gt; instance.</returns>
        public static Either<TLeft, TRight> ToEither<TLeft, TRight>(this TRight? value, Func<TLeft> exceptionFactory)
            where TRight : struct
        {
            if (exceptionFactory == null) throw new ArgumentNullException(nameof(exceptionFactory));
            return value.HasValue
                ? Either.Right<TLeft, TRight>(value.Value)
                : Either.Left<TLeft, TRight>(exceptionFactory());
        }

        /// <summary>
        /// Returns the existing value if present, or the attached 
        /// exceptional value.
        /// </summary>
        /// <param name="option">The specified optional.</param>
        /// <returns>The existing or exceptional value.</returns>
        public static T RightOrException<T>(this Either<T, T> option)
        {
            switch (option.State)
            {
                case EitherState.Bottom:
                    throw new BottomException();
                case EitherState.Right:
                    return option.Right;
                    break;
                case EitherState.Left:
                    return option.Left;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}