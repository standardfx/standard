using System;
using System.Collections.Generic;
using System.Linq;

namespace Standard.Data.Parsing
{
    partial class Parse
    {
        /// <summary>
        /// Fails on the first failure, if it reads at least one character.
        /// </summary>
        /// <exception cref="ArgumentNullException">Any of the arguments is <c>null</c>.</exception>
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
        /// Fails on the first failure, if it reads at least one character.
        /// </summary>
        /// <exception cref="ArgumentNullException">Any of the arguments is <c>null</c>.</exception>
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
        /// Fails if the parser is unable to repeatedly succeed for the number of times that is specified 
        /// by <paramref name="count" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">Any of the arguments is <c>null</c>.</exception>
        public static Parser<IEnumerable<T>> Repeat<T>(this Parser<T> parser, int count)
        {
            return Repeat(parser, count, count);
        }

        /// <summary>
        /// Fails if the parser is unable to repeatedly succeed for the number of times that is equal or greater 
        /// to <paramref name="minimumCount" />, and less than or equal to <paramref name="maximumCount" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">Any of the arguments is <c>null</c>.</exception>
        public static Parser<IEnumerable<T>> Repeat<T>(this Parser<T> parser, int minimumCount, int maximumCount)
        {
            if (parser == null) 
                throw new ArgumentNullException(nameof(parser));

            return i =>
            {
                IInput remainder = i;
                var result = new List<T>();

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
        /// Returns the item between <paramref name="open" /> and <paramref name="close" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">Any of the arguments is <c>null</c>.</exception>
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
