using System;
using System.Collections.Generic;

namespace Optional
{
    /// <summary>
    /// Represents an optional value, along with a potential exceptional value.
    /// </summary>
    /// <typeparam name="TRight">The type of the value to be wrapped.</typeparam>
    /// <typeparam name="TLeft">A exceptional value describing the lack of an actual value.</typeparam>
#if !NETSTANDARD10
    [Serializable]
#endif
    public struct Either<TLeft, TRight> : IEquatable<Either<TLeft, TRight>>, IComparable<Either<TLeft, TRight>>
    {
        private readonly EitherState state;
        private readonly TRight right;
        private readonly TLeft left;

        internal TLeft Left => left;
        internal TRight Right => right;

        internal EitherState State => state;

        internal Either(TRight right, TLeft left, EitherState state)
        {
            this.right = right;
            this.state = state;
            this.left = left;
        }

        /// <summary>
        /// Determines whether two eithers are equal.
        /// </summary>
        /// <param name="other">The optional to compare with the current one.</param>
        /// <returns>A boolean indicating whether or not the optionals are equal.</returns>
        public bool Equals(Either<TLeft, TRight> other)
        {
            if (state == other.state)
            {
                switch (state)
                {
                    case EitherState.Left:
                        return EqualityComparer<TLeft>.Default.Equals(Left, other.Left);
                    case EitherState.Right:
                        return EqualityComparer<TRight>.Default.Equals(right, other.Right);
                    case EitherState.Bottom:
                        return true;
                }
            }

            return false;
        }

        public bool IsRight =>
            State == EitherState.Right;

        /// <summary>
        /// Is the Either in a Left state?
        /// </summary>
        public bool IsLeft =>
            State == EitherState.Left;

        public bool IsBottom =>
            State == EitherState.Bottom;

        /// <summary>
        /// Determines whether two optionals are equal.
        /// </summary>
        /// <param name="obj">The optional to compare with the current one.</param>
        /// <returns>A boolean indicating whether or not the optionals are equal.</returns>
        public override bool Equals(object obj) =>
            obj is Either<TLeft, TRight> ? Equals((Either<TLeft, TRight>) obj) : false;

        /// <summary>
        /// Determines whether two optionals are equal.
        /// </summary>
        /// <param name="left">The first optional to compare.</param>
        /// <param name="right">The second optional to compare.</param>
        /// <returns>A boolean indicating whether or not the optionals are equal.</returns>
        public static bool operator ==(Either<TLeft, TRight> left, Either<TLeft, TRight> right) => left.Equals(right);

        /// <summary>
        /// Determines whether two optionals are unequal.
        /// </summary>
        /// <param name="left">The first optional to compare.</param>
        /// <param name="right">The second optional to compare.</param>
        /// <returns>A boolean indicating whether or not the optionals are unequal.</returns>
        public static bool operator !=(Either<TLeft, TRight> left, Either<TLeft, TRight> right) => !left.Equals(right);

        /// <summary>
        /// Generates a hash code for the current optional.
        /// </summary>
        /// <returns>A hash code for the current optional.</returns>
        public override int GetHashCode()
        {
            switch (state)
            {
                case EitherState.Right:
                    return right == null ? 2: right.GetHashCode();
                case EitherState.Left:
                    return left == null ? 1: left.GetHashCode();
                default:
                    return 0;
            }
        }


        /// <summary>
        /// Compares the relative order of two optionals. An empty optional is
        /// ordered by its exceptional value and always before a non-empty one.
        /// </summary>
        /// <param name="other">The optional to compare with the current one.</param>
        /// <returns>An integer indicating the relative order of the optionals being compared.</returns>
        public int CompareTo(Either<TLeft, TRight> other)
        {
            if (IsRight && !other.IsRight) return 1;
            if (!IsRight && other.IsRight) return -1;

            return IsRight
                ? Comparer<TRight>.Default.Compare(right, other.right)
                : Comparer<TLeft>.Default.Compare(left, other.left);
        }

        /// <summary>
        /// Determines if an optional is less than another optional.
        /// </summary>
        /// <param name="left">The first optional to compare.</param>
        /// <param name="right">The second optional to compare.</param>
        /// <returns>A boolean indicating whether or not the left optional is less than the right optional.</returns>
        public static bool operator <(Either<TLeft, TRight> left, Either<TLeft, TRight> right) =>
            left.CompareTo(right) < 0;

        /// <summary>
        /// Determines if an optional is less than or equal to another optional.
        /// </summary>
        /// <param name="left">The first optional to compare.</param>
        /// <param name="right">The second optional to compare.</param>
        /// <returns>A boolean indicating whether or not the left optional is less than or equal the right optional.</returns>
        public static bool operator <=(Either<TLeft, TRight> left, Either<TLeft, TRight> right) =>
            left.CompareTo(right) <= 0;

        /// <summary>
        /// Determines if an optional is greater than another optional.
        /// </summary>
        /// <param name="left">The first optional to compare.</param>
        /// <param name="right">The second optional to compare.</param>
        /// <returns>A boolean indicating whether or not the left optional is greater than the right optional.</returns>
        public static bool operator >(Either<TLeft, TRight> left, Either<TLeft, TRight> right) =>
            left.CompareTo(right) > 0;

        /// <summary>
        /// Determines if an optional is greater than or equal to another optional.
        /// </summary>
        /// <param name="left">The first optional to compare.</param>
        /// <param name="right">The second optional to compare.</param>
        /// <returns>A boolean indicating whether or not the left optional is greater than or equal the right optional.</returns>
        public static bool operator >=(Either<TLeft, TRight> left, Either<TLeft, TRight> right) =>
            left.CompareTo(right) >= 0;

        /// <summary>
        /// Returns a string that represents the current optional.
        /// </summary>
        /// <returns>A string that represents the current optional.</returns>
        public override string ToString()
        {
            switch (state)
            {
                case EitherState.Right:
                    if (right == null)
                    {
                        return "Right(null)";
                    }

                    return $"Right({right})";
                case EitherState.Left:
                    if (left == null)
                    {
                        return "Left(null)";
                    }

                    return $"Left({left})";
                case EitherState.Bottom:
                    return "Bottom()";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Converts the current optional into an enumerable with one or zero elements.
        /// </summary>
        /// <returns>A corresponding enumerable.</returns>
        public IEnumerable<TRight> ToEnumerable()
        {
            if (IsRight)
            {
                yield return right;
            }
        }

        /// <summary>
        /// Returns an enumerator for the optional.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<TRight> GetEnumerator()
        {
            if (IsRight)
            {
                yield return right;
            }
        }

        /// <summary>
        /// Determines if the current optional contains a specified value.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <returns>A boolean indicating whether or not the value was found.</returns>
        public bool Contains(TRight value)
        {
            if (IsRight)
            {
                if (this.right == null)
                {
                    return value == null;
                }

                return this.right.Equals(value);
            }

            return false;
        }

        /// <summary>
        /// Determines if the current optional contains a value 
        /// satisfying a specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A boolean indicating whether or not the predicate was satisfied.</returns>
        public bool Exists(Func<TRight, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return IsRight && predicate(right);
        }

        /// <summary>
        /// Returns the existing value if present, and otherwise an alternative value.
        /// </summary>
        /// <param name="alternative">The alternative value.</param>
        /// <returns>The existing or alternative value.</returns>
        public TRight RightOr(TRight alternative) => IsRight ? right : alternative;

        /// <summary>
        /// Returns the existing value if present, and otherwise an alternative value.
        /// </summary>
        /// <param name="alternativeFactory">A factory function to create an alternative value.</param>
        /// <returns>The existing or alternative value.</returns>
        public TRight RightOr(Func<TRight> alternativeFactory)
        {
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));
            return IsRight ? right : alternativeFactory();
        }

        /// <summary>
        /// Returns the existing value if present, and otherwise an alternative value.
        /// </summary>
        /// <param name="alternativeFactory">A factory function to map the exceptional value to an alternative value.</param>
        /// <returns>The existing or alternative value.</returns>
        public TRight RightOr(Func<TLeft, TRight> alternativeFactory)
        {
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));
            return IsRight ? right : alternativeFactory(left);
        }

        /// <summary>
        /// Uses an alternative value, if no existing value is present.
        /// </summary>
        /// <param name="alternative">The alternative value.</param>
        /// <returns>A new optional, containing either the existing or alternative value.</returns>
        public Either<TLeft, TRight> Or(TRight alternative) =>
            IsRight ? this : Either.Right<TLeft, TRight>(alternative);

        /// <summary>
        /// Uses an alternative value, if no existing value is present.
        /// </summary>
        /// <param name="alternativeFactory">A factory function to create an alternative value.</param>
        /// <returns>A new optional, containing either the existing or alternative value.</returns>
        public Either<TLeft, TRight> Or(Func<TRight> alternativeFactory)
        {
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));
            return IsRight ? this : Either.Right<TLeft, TRight>(alternativeFactory());
        }

        /// <summary>
        /// Uses an alternative value, if no existing value is present.
        /// </summary>
        /// <param name="alternativeFactory">A factory function to map the exceptional value to an alternative value.</param>
        /// <returns>A new optional, containing either the existing or alternative value.</returns>
        public Either<TLeft, TRight> Or(Func<TLeft, TRight> alternativeFactory)
        {
            if (alternativeFactory == null) throw new ArgumentNullException(nameof(alternativeFactory));
            return IsRight ? this : Either.Right<TLeft, TRight>(alternativeFactory(left));
        }

        /// <summary>
        /// Uses an alternative optional, if no existing value is present.
        /// </summary>
        /// <param name="alternativeOption">The alternative optional.</param>
        /// <returns>The alternative optional, if no value is present, otherwise itself.</returns>
        public Either<TLeft, TRight> Else(Either<TLeft, TRight> alternativeOption) =>
            IsRight ? this : alternativeOption;

        /// <summary>
        /// Uses an alternative optional, if no existing value is present.
        /// </summary>
        /// <param name="alternativeOptionFactory">A factory function to create an alternative optional.</param>
        /// <returns>The alternative optional, if no value is present, otherwise itself.</returns>
        public Either<TLeft, TRight> Else(Func<Either<TLeft, TRight>> alternativeOptionFactory)
        {
            if (alternativeOptionFactory == null) throw new ArgumentNullException(nameof(alternativeOptionFactory));
            return IsRight ? this : alternativeOptionFactory();
        }

        /// <summary>
        /// Uses an alternative optional, if no existing value is present.
        /// </summary>
        /// <param name="alternativeOptionFactory">A factory function to map the exceptional value to an alternative optional.</param>
        /// <returns>The alternative optional, if no value is present, otherwise itself.</returns>
        public Either<TLeft, TRight> Else(Func<TLeft, Either<TLeft, TRight>> alternativeOptionFactory)
        {
            if (alternativeOptionFactory == null) throw new ArgumentNullException(nameof(alternativeOptionFactory));
            return IsRight ? this : alternativeOptionFactory(left);
        }

        /// <summary>
        /// Forgets any attached exceptional value.
        /// </summary>
        /// <returns>An optional without an exceptional value.</returns>
        public Option<TRight> ToOption()
        {
            return Match(
                right: value => Option.Some(value),
                left: _ => Option.None<TRight>(),
                bottom: () => Option.None<TRight>()
            );
        }

        /// <summary>
        /// Evaluates a specified function, based on whether a value is present or not.
        /// </summary>
        /// <param name="right">The function to evaluate if the value is present.</param>
        /// <param name="left">The function to evaluate if the value is missing.</param>
        /// <param name="bottom">The function to evaluate if the either is in a Bottom state. Throws BottomException
        /// if no function is passed in.</param>
        /// <returns>The result of the evaluated function.</returns>
        public TResult Match<TResult>(Func<TRight, TResult> right, Func<TLeft, TResult> left,
            Func<TResult> bottom = null)
        {
            if (right == null) throw new ArgumentNullException(nameof(right));
            if (left == null) throw new ArgumentNullException(nameof(left));
            switch (state)
            {
                case EitherState.Right:
                    return right(this.right);
                case EitherState.Left:
                    return left(this.left);
                default:
                    return bottom == null ? throw new BottomException() : bottom();
            }
        }

        /// <summary>
        /// Evaluates a specified action, based on whether a value is present or not.
        /// </summary>
        /// <param name="right">The action to evaluate if the value is present.</param>
        /// <param name="left">The action to evaluate if the value is missing.</param>
        public void Match(Action<TRight> right, Action<TLeft> left)
        {
            if (right == null) throw new ArgumentNullException(nameof(right));
            if (left == null) throw new ArgumentNullException(nameof(left));

            if (IsRight)
            {
                right(Right);
            }
            else
            {
                left(Left);
            }
        }

        /// <summary>
        /// Evaluates a specified action if a value is present.
        /// </summary>
        /// <param name="some">The action to evaluate if the value is present.</param>
        public void MatchSome(Action<TRight> some)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));

            if (IsRight)
            {
                some(right);
            }
        }

        /// <summary>
        /// Evaluates a specified action if no value is present.
        /// </summary>
        /// <param name="none">The action to evaluate if the value is missing.</param>
        public void MatchNone(Action<TLeft> none)
        {
            if (none == null) throw new ArgumentNullException(nameof(none));

            if (!IsRight)
            {
                none(left);
            }
        }

        /// <summary>
        /// Transforms the inner value in an optional.
        /// If the instance is empty, an empty optional is returned.
        /// </summary>
        /// <param name="mapping">The transformation function.</param>
        /// <returns>The transformed optional.</returns>
        public Either<TLeft, TResult> Map<TResult>(Func<TRight, TResult> mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            return Match(
                right: value => Either.Right<TLeft, TResult>(mapping(value)),
                left: exception => Either.Left<TLeft, TResult>(exception)
            );
        }

        /// <summary>
        /// Transforms the exceptional value in an optional.
        /// If the instance is not empty, no transformation is carried out.
        /// </summary>
        /// <param name="mapping">The transformation function.</param>
        /// <returns>The transformed optional.</returns>
        public Either<TResult, TRight> MapLeft<TResult>(Func<TLeft, TResult> mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            return Match(
                right: value => Either.Right<TResult, TRight>(value),
                left: exception => Either.Left<TResult, TRight>(mapping(exception))
            );
        }

        /// <summary>
        /// Transforms the inner value in an optional
        /// into another optional. The result is flattened, 
        /// and if either is empty, an empty optional is returned.
        /// </summary>
        /// <param name="mapping">The transformation function.</param>
        /// <returns>The transformed optional.</returns>
        public Either<TLeft, TResult> FlatMap<TResult>(Func<TRight, Either<TLeft, TResult>> mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            return Match(
                right: mapping,
                left: Either.Left<TLeft, TResult>
            );
        }

        /// <summary>
        /// Transforms the inner value in an optional
        /// into another optional. The result is flattened, 
        /// and if either is empty, an empty optional is returned.
        /// </summary>
        /// <param name="mapping">The transformation function.</param>
        /// <returns>The transformed optional.</returns>
        public Either<TResult, TRight> FlatMapLeft<TResult>(Func<TLeft, Either<TResult, TRight>> mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            return Match(
                left: mapping,
                right: Either.Right<TResult, TRight>
            );
        }


        /// <summary>
        /// Empties an optional, and attaches an exceptional value, 
        /// if a specified condition is not satisfied.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="exception">The exceptional value to attach.</param>
        /// <returns>The filtered optional.</returns>
        public Either<TLeft, TRight> Filter(bool condition, TLeft exception) =>
            IsRight && !condition ? Either.Left<TLeft, TRight>(exception) : this;

        /// <summary>
        /// Empties an optional, and attaches an exceptional value, 
        /// if a specified condition is not satisfied.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="exceptionFactory">A factory function to create an exceptional value to attach.</param>
        /// <returns>The filtered optional.</returns>
        public Either<TLeft, TRight> Filter(bool condition, Func<TLeft> exceptionFactory)
        {
            if (exceptionFactory == null) throw new ArgumentNullException(nameof(exceptionFactory));
            return IsRight && !condition ? Either.Left<TLeft, TRight>(exceptionFactory()) : this;
        }

        /// <summary>
        /// Empties an optional, and attaches an exceptional value, 
        /// if a specified predicate is not satisfied.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="exception">The exceptional value to attach.</param>
        /// <returns>The filtered optional.</returns>
        public Either<TLeft, TRight> Filter(Func<TRight, bool> predicate, TLeft exception)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return IsRight && !predicate(right) ? Either.Left<TLeft, TRight>(exception) : this;
        }

        /// <summary>
        /// Empties an optional, and attaches an exceptional value, 
        /// if a specified predicate is not satisfied.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="exceptionFactory">A factory function to create an exceptional value to attach.</param>
        /// <returns>The filtered optional.</returns>
        public Either<TLeft, TRight> Filter(Func<TRight, bool> predicate, Func<TLeft> exceptionFactory)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (exceptionFactory == null) throw new ArgumentNullException(nameof(exceptionFactory));
            return IsRight && !predicate(right) ? Either.Left<TLeft, TRight>(exceptionFactory()) : this;
        }

        /// <summary>
        /// Empties an optional, and attaches an exceptional value, 
        /// if the value is null.
        /// </summary>
        /// <param name="exception">The exceptional value to attach.</param>
        /// <returns>The filtered optional.</returns>
        public Either<TLeft, TRight> NotNull(TLeft exception) =>
            IsRight && right == null ? Either.Left<TLeft, TRight>(exception) : this;

        /// <summary>
        /// Empties an optional, and attaches an exceptional value, 
        /// if the value is null.
        /// </summary>
        /// <param name="exceptionFactory">A factory function to create an exceptional value to attach.</param>
        /// <returns>The filtered optional.</returns>
        public Either<TLeft, TRight> NotNull(Func<TLeft> exceptionFactory)
        {
            if (exceptionFactory == null) throw new ArgumentNullException(nameof(exceptionFactory));
            return IsRight && right == null ? Either.Left<TLeft, TRight>(exceptionFactory()) : this;
        }
    }

    /// <summary>
    /// Provides a set of functions for creating optional values.
    /// </summary>
    public static class Either
    {
        public static Either<TLeft, TRight> Right<TLeft, TRight>(TRight value) =>
            new Either<TLeft, TRight>(value, default, EitherState.Right);


        public static Either<TLeft, TRight> Left<TLeft, TRight>(TLeft value) =>
            new Either<TLeft, TRight>(default, value, EitherState.Left);
    }

    public class BottomException : Exception
    {
    }

    internal enum EitherState
    {
        Bottom,
        Right,
        Left,
    }
}