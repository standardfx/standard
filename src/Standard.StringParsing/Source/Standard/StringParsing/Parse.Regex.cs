using System;
using System.Text.RegularExpressions;

namespace Standard.StringParsing
{
    partial class Parse
    {
        /// <summary>
        /// Creates a parser from the given regular expression.
        /// </summary>
        /// <param name="pattern">The regex expression.</param>
        /// <param name="description">Description of characters that do not match.</param>
        /// <returns>
        /// A parser of string.
        /// </returns>
        public static Parser<string> Regex(string pattern, string description = null)
        {
            if (pattern == null) 
                throw new ArgumentNullException(nameof(pattern));

            return Regex(new Regex(pattern), description);
        }

        /// <summary>
        /// Creates a parser from the given regular expression.
        /// </summary>
        /// <param name="regex">The regex expression.</param>
        /// <param name="description">Description of characters that do not match.</param>
        /// <returns>
        /// A parser of string.
        /// </returns>
        public static Parser<string> Regex(Regex regex, string description = null)
        {
            if (regex == null) 
                throw new ArgumentNullException(nameof(regex));

            return RegexMatch(regex, description).Then(match => Return(match.Value));
        }

        /// <summary>
        /// Creates a parser from the given regular expression, returning a parser of
        /// type <see cref="Match"/>.
        /// </summary>
        /// <param name="pattern">The regular expression.</param>
        /// <param name="description">Description of characters that do not match.</param>
        /// <returns>
        /// A parser of regular expression <see cref="Match"/> objects.
        /// </returns>
        public static Parser<Match> RegexMatch(string pattern, string description = null)
        {
            if (pattern == null) 
                throw new ArgumentNullException(nameof(pattern));

            return RegexMatch(new Regex(pattern), description);
        }

        /// <summary>
        /// Creates a parser from the specified regular expression, returning a parser of
        /// type <see cref="Match"/>.
        /// </summary>
        /// <param name="regex">The regular expression.</param>
        /// <param name="description">Description of characters that do not match.</param>
        /// <returns>
        /// A parser of regular expression <see cref="Match"/> objects.
        /// </returns>
        public static Parser<Match> RegexMatch(Regex regex, string description = null)
        {
            if (regex == null) 
                throw new ArgumentNullException(nameof(regex));

            regex = OptimizeRegex(regex);

            string[] expectations = description == null
                ? new string[0]
                : new[] { description };

            return i =>
            {
                if (!i.AtEnd)
                {
                    IInput remainder = i;
                    string input = i.Source.Substring(i.Position);
                    Match match = regex.Match(input);

                    if (match.Success)
                    {
                        for (int j = 0; j < match.Length; j++)
                            remainder = remainder.Advance();

                        return Result.Success(match, remainder);
                    }

                    string found = match.Index == input.Length
                        ? RS.EndOfSource
                        : string.Format("`{0}'", input[match.Index]);
                    
                    return Result.Failure<Match>(
                        remainder,
                        string.Format(RS.RegexMismatch, regex, found),
                        expectations);
                }

                return Result.Failure<Match>(i, RS.UnexpectedEndOfInput, expectations);
            };
        }

        /// <summary>
        /// Optimize the regex by only matching successfully at the start of the input.
        /// Do this by wrapping the whole regex in non-capturing parentheses preceded by
        ///  a `^'.
        /// </summary>
        /// <remarks>
        /// This method is invoked via reflection in unit tests. If renamed, the tests
        /// will need to be modified or they will fail.
        /// </remarks>
        private static Regex OptimizeRegex(Regex regex)
        {
            return new Regex(string.Format("^(?:{0})", regex), regex.Options);
        }
    }
}
