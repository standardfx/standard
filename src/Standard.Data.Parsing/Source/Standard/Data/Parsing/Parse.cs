using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Standard.Data.Parsing
{
    /// <summary>
    /// Parsers and combinators.
    /// </summary>
    public static partial class Parse
    {
        /// <summary>
        /// Parse a single character matching the <paramref name="predicate" /> specified.
        /// </summary>
        public static Parser<char> Char(Predicate<char> predicate, string description)
        {
            if (predicate == null) 
                throw new ArgumentNullException(nameof(predicate));
            if (description == null) 
                throw new ArgumentNullException(nameof(description));

            return i =>
            {
                if (!i.AtEnd)
                {
                    if (predicate(i.Current))
                        return Result.Success(i.Current, i.Advance());

                    return Result.Failure<char>(i,
                        string.Format(RS.UnexpectedToken, i.Current),
                        new[] { description });
                }

                return Result.Failure<char>(i,
                    RS.UnexpectedEndOfInput,
                    new[] { description });
            };
        }

        /// <summary>
        /// Parse a single character matching the <paramref name="predicate" /> specified.
        /// </summary>
        /// <param name="predicate">Characters not to match.</param>
        /// <param name="description">Description of characters that don't match.</param>
        /// <returns>A parser for characters except those matching <paramref name="predicate"/>.</returns>
        public static Parser<char> CharExcept(Predicate<char> predicate, string description)
        {
            return Char(c => !predicate(c), "any character except " + description);
        }

        /// <summary>
        /// Parse a single character.
        /// </summary>
        public static Parser<char> Char(char c)
        {
            return Char(ch => c == ch, char.ToString(c));
        }


        /// <summary>
        /// Parses a single character of any in the given parameter.
        /// </summary>
        public static Parser<char> Chars(params char[] c)
        {
            return Char(c.Contains, StringHelper.Join("|", c));
        }

        /// <summary>
        /// Parses a single character of any in the given parameter.
        /// </summary>
        public static Parser<char> Chars(string c)
        {
            return Char(c.ToEnumerable().Contains, StringHelper.Join("|", c.ToEnumerable()));
        }


        /// <summary>
        /// Parses a single character except for those in the given parameter.
        /// </summary>
        public static Parser<char> CharExcept(char c)
        {
            return CharExcept(ch => c == ch, char.ToString(c));
        }

        /// <summary>
        /// Parses a single character except for those in the given parameter.
        /// </summary>
        public static Parser<char> CharExcept(IEnumerable<char> c)
        {
            char[] chars = c as char[] ?? c.ToArray();
            return CharExcept(chars.Contains, StringHelper.Join("|", chars));
        }

        /// <summary>
        /// Parses a single character except for those in the given parameter.
        /// </summary>  
        public static Parser<char> CharExcept(string c)
        {
            return CharExcept(c.ToEnumerable().Contains, StringHelper.Join("|", c.ToEnumerable()));
        }

        /// <summary>
        /// Performs case-insensitive parsing on a single character.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Parser<char> IgnoreCase(char c)
        {
            return Char(ch => char.ToLower(c) == char.ToLower(ch), char.ToString(c));
        }

        /// <summary>
        /// Performs case-insensitive parsing of the string specified.
        /// </summary>
        public static Parser<IEnumerable<char>> IgnoreCase(string s)
        {
            if (s == null) 
                throw new ArgumentNullException(nameof(s));

            return s
                .ToEnumerable()
                .Select(IgnoreCase)
                .Aggregate(Return(Enumerable.Empty<char>()),
                    (a, p) => a.Concat(p.Once()))
                .Named(s);
        }

        /// <summary>
        /// Parse any character.
        /// </summary>
        public static readonly Parser<char> AnyChar = Char(c => true, "any character");

        /// <summary>
        /// Parse a whitespace.
        /// </summary>
        public static readonly Parser<char> WhiteSpace = Char(char.IsWhiteSpace, "whitespace");

        /// <summary>
        /// Parse a digit.
        /// </summary>
        public static readonly Parser<char> Digit = Char(char.IsDigit, "digit");

        /// <summary>
        /// Parse a letter.
        /// </summary>
        public static readonly Parser<char> Letter = Char(char.IsLetter, "letter");

        /// <summary>
        /// Parse a letter or digit.
        /// </summary>
        public static readonly Parser<char> LetterOrDigit = Char(char.IsLetterOrDigit, "letter or digit");

        /// <summary>
        /// Parse a lowercase letter.
        /// </summary>
        public static readonly Parser<char> Lower = Char(char.IsLower, "lowercase letter");
        
        /// <summary>
        /// Parse an uppercase letter.
        /// </summary>
        public static readonly Parser<char> Upper = Char(char.IsUpper, "uppercase letter");

        /// <summary>
        /// Parse a numeric character.
        /// </summary>
        public static readonly Parser<char> Numeric = Char(char.IsNumber, "numeric character");

        /// <summary>
        /// Parse a string of characters.
        /// </summary>
        public static Parser<IEnumerable<char>> String(string s)
        {
            if (s == null) 
                throw new ArgumentNullException(nameof(s));

            return s
                .ToEnumerable()
                .Select(Char)
                .Aggregate(Return(Enumerable.Empty<char>()),
                    (a, p) => a.Concat(p.Once()))
                .Named(s);
        }

        /// <summary>
        /// Constructs a parser that will fail if the given parser succeeds,
        /// and will succeed if the given parser fails. In any case, it does not 
        /// consume any input. It is similar to negative look-ahead in regex.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser</typeparam>
        /// <param name="parser">The parser to wrap</param>
        /// <returns>A parser that is the opposite of the given parser.</returns>
        public static Parser<object> Not<T>(this Parser<T> parser)
        {
            if (parser == null) 
                throw new ArgumentNullException(nameof(parser));

            return i =>
            {
                var result = parser(i);

                if (result.WasSuccessful)
                {
                    string msg = string.Format(RS.UnexpectedToken, StringHelper.Join(", ", result.Expectations));
                    return Result.Failure<object>(i, msg, new string[0]);
                }

                return Result.Success<object>(null, i);
            };
        }
 
        /// <summary>
        /// Parse first, and if successful, then parse second.
        /// </summary>
        public static Parser<U> Then<T, U>(this Parser<T> first, Func<T, Parser<U>> second)
        {
            if (first == null) 
                throw new ArgumentNullException(nameof(first));
            if (second == null) 
                throw new ArgumentNullException(nameof(second));

            return i => first(i).IfSuccess(s => second(s.Value)(s.Remainder));
        }

        /// <summary>
        /// Parse a stream of elements.
        /// </summary>
        /// <remarks>
        /// This function is implemented imperatively to decrease stack usage.
        /// </remarks>
        public static Parser<IEnumerable<T>> Many<T>(this Parser<T> parser)
        {
            if (parser == null) 
                throw new ArgumentNullException(nameof(parser));

            return i =>
            {
                IInput remainder = i;
                var result = new List<T>();
                var r = parser(i);

                while (r.WasSuccessful)
                {
                    if (remainder.Equals(r.Remainder))
                        break;

                    result.Add(r.Value);
                    remainder = r.Remainder;
                    r = parser(remainder);
                }

                return Result.Success<IEnumerable<T>>(result, remainder);
            };
        }

        /// <summary>
        /// Parse a stream of elements, failing if any element is only partially parsed.
        /// </summary>
        /// <typeparam name="T">The type of element to parse.</typeparam>
        /// <param name="parser">A parser that matches a single element.</param>
        /// <returns>A <see cref="Parser{T}"/> that matches the sequence.</returns>
        /// <remarks>
        /// <para>
        /// Using <seealso cref="XMany{T}(Parser{T})"/> may be preferable to <seealso cref="Many{T}(Parser{T})"/>
        /// where the first character of each match identified by <paramref name="parser"/>
        /// is sufficient to determine whether the entire match should succeed. The X*
        /// methods typically give more helpful errors and are easier to debug than their
        /// unqualified counterparts.
        /// </para>
        /// </remarks>
        /// <seealso cref="XOr"/>
        public static Parser<IEnumerable<T>> XMany<T>(this Parser<T> parser)
        {
            if (parser == null) 
                throw new ArgumentNullException(nameof(parser));

            return parser.Many().Then(m => parser.Once().XOr(Return(m)));
        }

        /// <summary>
        /// Try to parse a stream of elements with at least one item.
        /// </summary>
        public static Parser<IEnumerable<T>> AtLeastOnce<T>(this Parser<T> parser)
        {
            if (parser == null) 
                throw new ArgumentNullException(nameof(parser));

            return parser.Once().Then(t1 => parser.Many().Select(ts => t1.Concat(ts)));
        }

        /// <summary>
        /// Try to parse a stream of elements with at least one item. Except the first
        /// item, all other items will be matched with the <code>XMany</code> operator.
        /// </summary>
        public static Parser<IEnumerable<T>> XAtLeastOnce<T>(this Parser<T> parser)
        {
            if (parser == null) 
                throw new ArgumentNullException(nameof(parser));

            return parser.Once().Then(t1 => parser.XMany().Select(ts => t1.Concat(ts)));
        }

        /// <summary>
        /// Parse end-of-input.
        /// </summary>
        public static Parser<T> End<T>(this Parser<T> parser)
        {
            if (parser == null) 
                throw new ArgumentNullException(nameof(parser));

            return i => parser(i).IfSuccess(s =>
                s.Remainder.AtEnd 
                    ? s
                    : Result.Failure<T>(
                        s.Remainder,
                        string.Format(RS.UnexpectedToken, s.Remainder.Current),
                        new[] { RS.EndOfInput }));
        }

        /// <summary>
        /// Take the result of parsing, and project it onto a different domain.
        /// </summary>
        public static Parser<U> Select<T, U>(this Parser<T> parser, Func<T, U> convert)
        {
            if (parser == null) 
                throw new ArgumentNullException(nameof(parser));
            if (convert == null) 
                throw new ArgumentNullException(nameof(convert));

            return parser.Then(t => Return(convert(t)));
        }

        /// <summary>
        /// Parse the token, embedded in any amount of whitespace characters.
        /// </summary>
        public static Parser<T> Token<T>(this Parser<T> parser)
        {
            if (parser == null) 
                throw new ArgumentNullException(nameof(parser));

            return 
                from leading in WhiteSpace.Many()
                from item in parser
                from trailing in WhiteSpace.Many()
                select item;
        }

        /// <summary>
        /// Refer to another parser indirectly. This allows circular compile-time dependency between parsers.
        /// </summary>
        public static Parser<T> Ref<T>(Func<Parser<T>> reference)
        {
            if (reference == null) 
                throw new ArgumentNullException(nameof(reference));

            Parser<T> p = null;

            return i =>
            {
                if (p == null)
                    p = reference();

                if (i.Memos.ContainsKey(p))
                    throw new ParseException(i.Memos[p].ToString());

                i.Memos[p] = Result.Failure<T>(i,
                    RS.LeftGrammerRecursion,
                    new string[0]);
               
                var result = p(i);
                i.Memos[p] = result;
                return result;
            };
        }

        /// <summary>
        /// Convert a stream of characters to a string.
        /// </summary>
        public static Parser<string> Text(this Parser<IEnumerable<char>> characters)
        {
            return characters.Select(chs => new string(chs.ToArray()));
        }

        /// <summary>
        /// Parse first, if it succeeds, return first, otherwise try second.
        /// </summary>
        public static Parser<T> Or<T>(this Parser<T> first, Parser<T> second)
        {
            if (first == null) 
                throw new ArgumentNullException(nameof(first));
            if (second == null) 
                throw new ArgumentNullException(nameof(second));

            return i =>
            {
                var fr = first(i);
                if (!fr.WasSuccessful)
                    return second(i).IfFailure(sf => DetermineBestError(fr, sf));
                
                if (fr.Remainder.Equals(i))
                    return second(i).IfFailure(sf => fr);

                return fr;
            };
        }

        /// <summary>
        /// Names part of the grammar for help with error messages.
        /// </summary>
        public static Parser<T> Named<T>(this Parser<T> parser, string name)
        {
            if (parser == null) 
                throw new ArgumentNullException(nameof(parser));
            if (name == null) 
                throw new ArgumentNullException(nameof(name));

            return i => 
                parser(i).IfFailure(f => f.Remainder.Equals(i) 
                    ? Result.Failure<T>(f.Remainder, f.Message, new[] { name }) 
                    : f);
        }

        /// <summary>
        /// Parse first, if it succeeds, return first, otherwise try second.
        /// </summary>
        /// <remarks>
        /// Assumes that the first parsed character will determine the parser chosen (see Try).
        /// </remarks>
        public static Parser<T> XOr<T>(this Parser<T> first, Parser<T> second)
        {
            if (first == null) 
                throw new ArgumentNullException(nameof(first));
            if (second == null) 
                throw new ArgumentNullException(nameof(second));

            return i => 
            {
                var fr = first(i);
                if (!fr.WasSuccessful)
                {
                    // The 'X' part
                    if (!fr.Remainder.Equals(i))
                        return fr; 
                    
                    return second(i).IfFailure(sf => DetermineBestError(fr, sf));
                }

                // This handles a zero-length successful application of first.
                if (fr.Remainder.Equals(i))
                    return second(i).IfFailure(sf => fr);

                return fr;
            };
        }

        /// <summary>
        /// Examines two results presumably obtained at an "Or" junction; returns the result with
        /// the most information, or if they apply at the same input position, a union of the results.
        /// </summary>
        private static IResult<T> DetermineBestError<T>(IResult<T> firstFailure, IResult<T> secondFailure)
        {
            if (secondFailure.Remainder.Position > firstFailure.Remainder.Position)
                return secondFailure;

            if (secondFailure.Remainder.Position == firstFailure.Remainder.Position)
                return Result.Failure<T>(
                    firstFailure.Remainder,
                    firstFailure.Message,
                    firstFailure.Expectations.Union(secondFailure.Expectations));

            return firstFailure;
        }

        /// <summary>
        /// Parse a stream of elements containing only one item.
        /// </summary>
        public static Parser<IEnumerable<T>> Once<T>(this Parser<T> parser)
        {
            if (parser == null) 
                throw new ArgumentNullException(nameof(parser));

            return parser.Select(r => (IEnumerable<T>)new[] { r });
        }

        /// <summary>
        /// Concatenate two streams of elements.
        /// </summary>
        public static Parser<IEnumerable<T>> Concat<T>(this Parser<IEnumerable<T>> first, Parser<IEnumerable<T>> second)
        {
            if (first == null) 
                throw new ArgumentNullException(nameof(first));
            if (second == null) 
                throw new ArgumentNullException(nameof(second));

            return first.Then(f => second.Select(f.Concat));
        }

        /// <summary>
        /// Succeed immediately and return value.
        /// </summary>
        public static Parser<T> Return<T>(T value)
        {
            return i => Result.Success(value, i);
        }

        /// <summary>
        /// Alternative Return function with simpler inline syntax.
        /// </summary>
        public static Parser<U> Return<T, U>(this Parser<T> parser, U value)
        {
            if (parser == null) 
                throw new ArgumentNullException(nameof(parser));
            
            return parser.Select(t => value);
        }

        /// <summary>
        /// Attempt parsing only if the <paramref name="except"/> parser fails.
        /// </summary>
        public static Parser<T> Except<T, U>(this Parser<T> parser, Parser<U> except)
        {
            if (parser == null) 
                throw new ArgumentNullException(nameof(parser));
            if (except == null) 
                throw new ArgumentNullException(nameof(except));

            // Could be more like: except.Then(s => s.Fail("..")).XOr(parser)
            return i =>
            {
                var r = except(i);
                if (r.WasSuccessful)
                    return Result.Failure<T>(i, RS.ExceptedParserSucceeded, new[] { RS.OtherThanExceptedInput });
                return parser(i);
            };
        }

        /// <summary>
        /// Parse a sequence of items until a terminator is reached.
        /// </summary>
        /// <returns>The sequence parsed, but excluding the terminator.</returns>
        public static Parser<IEnumerable<T>> Until<T, U>(this Parser<T> parser, Parser<U> until)
        {
            return parser.Except(until).Many().Then(r => until.Return(r));
        }

        /// <summary>
        /// Succeeds if the parsed value matches the specified predicate.
        /// </summary>
        public static Parser<T> Where<T>(this Parser<T> parser, Func<T, bool> predicate)
        {
            if (parser == null) 
                throw new ArgumentNullException(nameof(parser));
            if (predicate == null) 
                throw new ArgumentNullException(nameof(predicate));

            return i => parser(i).IfSuccess(s =>
                predicate(s.Value) 
                    ? s 
                    : Result.Failure<T>(i, string.Format("Unexpected {0}.", s.Value), new string[0]));
        }

        /// <summary>
        /// Monadic combinator Then, adapted for Linq comprehension syntax.
        /// </summary>
        public static Parser<V> SelectMany<T, U, V>(this Parser<T> parser, Func<T, Parser<U>> selector, Func<T, U, V> projector)
        {
            if (parser == null) 
                throw new ArgumentNullException(nameof(parser));
            if (selector == null) 
                throw new ArgumentNullException(nameof(selector));
            if (projector == null) 
                throw new ArgumentNullException(nameof(projector));

            return parser.Then(t => selector(t).Select(u => projector(t, u)));
        }

        /// <summary>
        /// Chain a left-associative operator.
        /// </summary>
        public static Parser<T> ChainOperator<T, TOp>(Parser<TOp> op, Parser<T> operand, Func<TOp, T, T, T> apply)
        {
            if (op == null) 
                throw new ArgumentNullException(nameof(op));
            if (operand == null) 
                throw new ArgumentNullException(nameof(operand));
            if (apply == null) 
                throw new ArgumentNullException(nameof(apply));
            
            return operand.Then(first => ChainOperatorRest(first, op, operand, apply, Or));
        }

        /// <summary>
        /// Chain a left-associative operator.
        /// </summary>
        public static Parser<T> XChainOperator<T, TOp>(Parser<TOp> op, Parser<T> operand, Func<TOp, T, T, T> apply)
        {
            if (op == null) 
                throw new ArgumentNullException(nameof(op));
            if (operand == null) 
                throw new ArgumentNullException(nameof(operand));
            if (apply == null) 
                throw new ArgumentNullException(nameof(apply));
            
            return operand.Then(first => ChainOperatorRest(first, op, operand, apply, XOr));
        }

        private static Parser<T> ChainOperatorRest<T, TOp>(T firstOperand, Parser<TOp> op, Parser<T> operand,
            Func<TOp, T, T, T> apply,
            Func<Parser<T>, Parser<T>, Parser<T>> or)
        {
            if (op == null) 
                throw new ArgumentNullException(nameof(op));
            if (operand == null) 
                throw new ArgumentNullException(nameof(operand));
            if (apply == null) 
                throw new ArgumentNullException(nameof(apply));

            return 
                or(op.Then(opvalue =>
                    operand.Then(operandValue =>
                        ChainOperatorRest(apply(opvalue, firstOperand, operandValue), op, operand, apply, or))),
                    Return(firstOperand));
        }

        /// <summary>
        /// Chain a right-associative operator.
        /// </summary>
        public static Parser<T> ChainRightOperator<T, TOp>(Parser<TOp> op, Parser<T> operand,
            Func<TOp, T, T, T> apply)
        {
            if (op == null) 
                throw new ArgumentNullException(nameof(op));
            if (operand == null) 
                throw new ArgumentNullException(nameof(operand));
            if (apply == null) 
                throw new ArgumentNullException(nameof(apply));

            return operand.Then(first => ChainRightOperatorRest(first, op, operand, apply, Or));
        }

        /// <summary>
        /// Chains a right-associative operator.
        /// </summary>
        public static Parser<T> XChainRightOperator<T, TOp>(Parser<TOp> op, Parser<T> operand, Func<TOp, T, T, T> apply)
        {
            if (op == null) 
                throw new ArgumentNullException(nameof(op));
            if (operand == null) 
                throw new ArgumentNullException(nameof(operand));
            if (apply == null) 
                throw new ArgumentNullException(nameof(apply));
            
            return operand.Then(first => ChainRightOperatorRest(first, op, operand, apply, XOr));
        }

        private static Parser<T> ChainRightOperatorRest<T, TOp>(T lastOperand, Parser<TOp> op, Parser<T> operand,
            Func<TOp, T, T, T> apply,
            Func<Parser<T>, Parser<T>, Parser<T>> or)
        {
            if (op == null) 
                throw new ArgumentNullException(nameof(op));
            if (operand == null) 
                throw new ArgumentNullException(nameof(operand));
            if (apply == null) 
                throw new ArgumentNullException(nameof(apply));
            
            return 
                or(op.Then(opvalue =>
                    operand.Then(operandValue =>
                        ChainRightOperatorRest(operandValue, op, operand, apply, or)).Then(r =>
                            Return(apply(opvalue, lastOperand, r)))),
                    Return(lastOperand));
        }

        /// <summary>
        /// Parse a number.
        /// </summary>
        public static readonly Parser<string> Number = Numeric.AtLeastOnce().Text();

        private static Parser<string> DecimalWithoutLeadingDigits(CultureInfo ci = null)
        {
            return 
                from nothing in Return(string.Empty)
                // dummy so that CultureInfo.CurrentCulture is evaluated later
                from dot in String((ci ?? CultureInfo.CurrentCulture).NumberFormat.NumberDecimalSeparator).Text()
                from fraction in Number
                select dot + fraction;
        }

        private static Parser<string> DecimalWithLeadingDigits(CultureInfo ci = null)
        {
            return Number.Then(n => DecimalWithoutLeadingDigits(ci).XOr(Return(string.Empty)).Select(f => n + f));
        }

        /// <summary>
        /// Parse a decimal number, using the current culture's separator character.
        /// </summary>
        public static readonly Parser<string> Decimal = 
            DecimalWithLeadingDigits().XOr(DecimalWithoutLeadingDigits());

        /// <summary>
        /// Parse a decimal number, using the separator '.'.
        /// </summary>
        public static readonly Parser<string> DecimalInvariant = 
            DecimalWithLeadingDigits(CultureInfo.InvariantCulture)
            .XOr(DecimalWithoutLeadingDigits(CultureInfo.InvariantCulture));
    }
}
