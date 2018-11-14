using System;

namespace Standard.StringParsing
{
    partial class Parse
    {
        /// <summary>
        /// Creates a parser that indicates the specified parser is optional. 
        /// The returned parser will succeed on any input no matter whether the specified parser
        /// succeeds or not.
        /// </summary>
        /// <typeparam name="T">The result type of the specified parser.</typeparam>
        /// <param name="parser">The parser to wrap.</param>
        /// <returns>
        /// An optional version of the specified parser.
        /// </returns>
        public static Parser<IOption<T>> Optional<T>(this Parser<T> parser)
        {
            if (parser == null) 
                throw new ArgumentNullException(nameof(parser));

            return i =>
            {
                IResult<T> pr = parser(i);

                if (pr.WasSuccessful)
                    return Result.Success(new Some<T>(pr.Value), pr.Remainder);

                return Result.Success(new None<T>(), i);
            };
        }

        /// <summary>
        /// Creates the eXclusive version of the <see cref="Optional{T}"/> parser.
        /// </summary>
        /// <typeparam name="T">The result type of the specified parser.</typeparam>
        /// <param name="parser">The parser to wrap.</param>
        /// <returns>
        /// An eXclusive optional version of the specified parser.
        /// </returns>
        /// <seealso cref="XOr"/>
        public static Parser<IOption<T>> XOptional<T>(this Parser<T> parser)
        {
            if (parser == null) 
                throw new ArgumentNullException(nameof(parser));

            return i =>
            {
                IResult<T> result = parser(i);

                if (result.WasSuccessful)
                    return Result.Success(new Some<T>(result.Value), result.Remainder);

                if (result.Remainder.Equals(i))
                    return Result.Success(new None<T>(), i);

                return Result.Failure<IOption<T>>(result.Remainder, result.Message, result.Expectations);
            };
        }

        /// <summary>
        /// Creates a parser that indicates that the specified parser is optional
        /// and non-consuming. The returned parser will succeed on
        /// any input no matter whether the specified parser succeeds or not.
        /// In any case, it will not consume any input, like a positive look-ahead in regular expression.
        /// </summary>
        /// <typeparam name="T">The result type of the specified parser.</typeparam>
        /// <param name="parser">The parser to wrap.</param>
        /// <returns>
        /// A non-consuming version of the specified parser.
        /// </returns>
        public static Parser<IOption<T>> Preview<T>(this Parser<T> parser)
        {
            if (parser == null) 
                throw new ArgumentNullException(nameof(parser));

            return i =>
            {
                IResult<T> result = parser(i);

                if (result.WasSuccessful)
                    return Result.Success(new Some<T>(result.Value), i);

                return Result.Success(new None<T>(), i);
            };
        }
    }
}
