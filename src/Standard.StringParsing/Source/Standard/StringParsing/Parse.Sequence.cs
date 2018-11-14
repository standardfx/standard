using System;
using System.Collections.Generic;
using System.Linq;

namespace Standard.StringParsing
{
    partial class Parse
    {
        /// <summary>
        /// Parse for a repeated pattern, separated by the delimiter specified.
        /// </summary>
        /// <typeparam name="T">The type of the matched result for <paramref name="parser"/>.</typeparam>
        /// <typeparam name="U">The type of the matched result for <paramref name="delimiter"/>.</typeparam>
        /// <param name="parser">A parser instance.</param>
        /// <param name="delimiter">A delimiter that separates each occurance of a repeated pattern.</param>
        /// <exception cref="ArgumentNullException">One or more of the arguments is `null`.</exception>
        /// <returns>
        /// A parser for a repeated pattern, separated by <paramref name="delimiter"/>.
        /// </returns>
        public static Parser<IEnumerable<T>> DelimitedBy<T, U>(this Parser<T> parser, Parser<U> delimiter)
        {
            if (parser == null) 
                throw new ArgumentNullException(nameof(parser));
            if (delimiter == null) 
                throw new ArgumentNullException(nameof(delimiter));

            return 
                from head in parser.Once()
                from tail in 
                    (
                        from separator in delimiter
                        from item in parser
                        select item
                    ).Many()
                select head.Concat(tail);
        }

        /// <summary>
        /// Parse for a repeated pattern, separated by the delimiter specified. This parser fails on the first failure.
        /// </summary>
        /// <typeparam name="T">The type of the matched result for <paramref name="parser"/>.</typeparam>
        /// <typeparam name="U">The type of the matched result for <paramref name="delimiter"/>.</typeparam>
        /// <param name="parser">A parser instance.</param>
        /// <param name="delimiter">A delimiter that separates each occurance of a repeated pattern.</param>
        /// <exception cref="ArgumentNullException">One or more of the arguments is `null`.</exception>
        /// <returns>
        /// A parser for a repeated pattern, separated by <paramref name="delimiter"/>.
        /// </returns>
        public static Parser<IEnumerable<T>> XDelimitedBy<T, U>(this Parser<T> parser, Parser<U> delimiter)
        {
            if (parser == null) 
                throw new ArgumentNullException(nameof(parser));
            if (delimiter == null) 
                throw new ArgumentNullException(nameof(delimiter));

            return 
                from head in parser.Once()
                from tail in
                    (
                        from separator in delimiter
                        from item in parser
                        select item
                    ).XMany()
                select head.Concat(tail);
        }

        /// <summary>
        /// Fails if the parser is unable to repeatedly succeed for the specified number of times.
        /// </summary>
        /// <typeparam name="T">The type of the matched result.</typeparam>
        /// <param name="parser">A parser instance.</param>
        /// <param name="count">The number of times that the parser should succeed.</param>
        /// <exception cref="ArgumentNullException">One or more of the arguments is `null`.</exception>
        /// <returns>
        /// A parser that will fail if it is unable to successfully repeat <paramref name="parser"/> for 
        /// the number of times specified by <paramref name="count"/>.
        /// </returns>
        public static Parser<IEnumerable<T>> Repeat<T>(this Parser<T> parser, int count)
        {
            return Repeat(parser, count, count);
        }

        /// <summary>
        /// Fails if the parser is unable to repeatedly succeed for the specified number of times.
        /// </summary>
        /// <typeparam name="T">The type of the matched result.</typeparam>
        /// <param name="parser">A parser instance.</param>
        /// <param name="minimumCount">The minimum number of times that the parser should succeed.</param>
        /// <param name="maximumCount">The maximum number of times that the parser may succeed.</param>
        /// <exception cref="ArgumentNullException">One or more of the arguments is `null`.</exception>
        /// <returns>
        /// A parser that will fail if it is unable to successfully repeat <paramref name="parser"/> for 
        /// at least <paramref name="minimumCount"/> times, but no more than <paramref name="maximumCount"/> 
        /// times.
        /// </returns>
        public static Parser<IEnumerable<T>> Repeat<T>(this Parser<T> parser, int minimumCount, int maximumCount)
        {
            if (parser == null) 
                throw new ArgumentNullException(nameof(parser));

            return i =>
            {
                IInput remainder = i;
                List<T> result = new List<T>();

                for (int n = 0; n < maximumCount; ++n)
                {
                    IResult<T> r = parser(remainder);

                    if (!r.WasSuccessful && n < minimumCount)
                    {
                        string what = r.Remainder.AtEnd
                            ? RS.EndOfInput
                            : r.Remainder.Current.ToString();

                        string msg = string.Format(RS.UnexpectedToken, what);
                        string exp = (minimumCount == maximumCount)
                            ? string.Format(RS.RepeatExactCountExpectation, StringHelper.Join(", ", r.Expectations), minimumCount, n)
                            : string.Format(RS.RepeatCountExpectation, StringHelper.Join(", ", r.Expectations), minimumCount, maximumCount, n);

                        return Result.Failure<IEnumerable<T>>(i, msg, new[] { exp });
                    }

                    if (!ReferenceEquals(remainder, r.Remainder))
                        result.Add(r.Value);

                    remainder = r.Remainder;
                }

                return Result.Success<IEnumerable<T>>(result, remainder);
            };
        }

        /// <summary>
        /// Returns the item between two tokens.
        /// </summary>
        /// <typeparam name="T">The type of the matched result for <paramref name="parser"/>.</typeparam>
        /// <typeparam name="U">The type of the matched result for <paramref name="open"/>.</typeparam>
        /// <typeparam name="V">The type of the matched result for <paramref name="close"/>.</typeparam>
        /// <param name="parser">This parser instance.</param>
        /// <param name="open">A parser for the token immediately before the item to be returned.</param>
        /// <param name="close">A parser for the token immediately after the item to be returned.</param>
        /// <exception cref="ArgumentNullException">One or more of the arguments is `null`.</exception>
        /// <returns>
        /// A parser for the item between <paramref name="open"/> and <paramref name="close"/>.
        /// </returns>
        public static Parser<T> Contained<T, U, V>(this Parser<T> parser, Parser<U> open, Parser<V> close)
        {
            if (parser == null) 
                throw new ArgumentNullException(nameof(parser));
            if (open == null) 
                throw new ArgumentNullException(nameof(open));
            if (close == null) 
                throw new ArgumentNullException(nameof(close));

            return 
                from o in open
                from item in parser
                from c in close
                select item;
        }
    }
}
