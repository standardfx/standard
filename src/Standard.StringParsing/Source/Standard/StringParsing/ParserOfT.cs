using System;

namespace Standard.StringParsing
{
    /// <summary>
    /// Represents a string parser.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="input">The input string to parse.</param>
    /// <returns>
    /// The result returned by the parser.
    /// </returns>
    public delegate IResult<T> Parser<out T>(IInput input);

    /// <summary>
    /// Contains extension methods for <see cref="Parser{T}" />.
    /// </summary>
    public static class ParserExtensions
    {
        /// <summary>
        /// Parses the specified input string.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="parser">The parser instance.</param>
        /// <param name="input">The input string to parse.</param>
        /// <exception cref="ArgumentNullException">One or more parameters is `null` (`Nothing` in Visual Basic).</exception>
        /// <returns>
        /// The result returned by the parser after parsing the source <paramref name="input"/>. Check the <see cref="IResult{T}.WasSuccessful"/> 
        /// property to determine whether the input was parsed successfully.
        /// </returns>
        public static IResult<T> TryParse<T>(this Parser<T> parser, string input)
        {
            if (parser == null) 
                throw new ArgumentNullException(nameof(parser));
            if (input == null) 
                throw new ArgumentNullException(nameof(input));

            return parser(new Input(input));
        }

        /// <summary>
        /// Parses the specified input string.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="parser">The parser instance.</param>
        /// <param name="input">The input string to parse.</param>
        /// <exception cref="ParseException">An error has occured while parsing the specified input string.</exception>
        /// <exception cref="ArgumentNullException">One or more parameters is `null` (`Nothing` in Visual Basic).</exception>
        /// <returns>
        /// The result returned by the parser after parsing the source <paramref name="input"/>.
        /// </returns>
        public static T Parse<T>(this Parser<T> parser, string input)
        {
            if (parser == null) 
                throw new ArgumentNullException(nameof(parser));
            if (input == null) 
                throw new ArgumentNullException(nameof(input));

            IResult<T> result = parser.TryParse(input);
            
            if (result.WasSuccessful)
                return result.Value;

            throw new ParseException(result.ToString());
        }
    }
}
