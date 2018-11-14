namespace Standard.StringParsing
{
    partial class Parse
    {
        /// <summary>
        /// Newline (`\n`) or carriage return and newline (`\r\n`).
        /// </summary>
        public static Parser<string> LineEnd =
            (
                from r in Char('\r').Optional()
                from n in Char('\n')
                select r.IsDefined 
                    ? r.Get().ToString() + n 
                    : n.ToString()
            ).Named("LineEnd");

        /// <summary>
        /// Line ending or end of input.
        /// </summary>
        public static Parser<string> LineTerminator =
            Return(string.Empty).End()
            .Or(LineEnd.End())
            .Or(LineEnd)
            .Named("LineTerminator");

        /// <summary>
        /// A parser for an identifier, starting with the specified first letter and continuing with trailing letters.
        /// </summary>
        /// <param name="firstLetterParser">A parser for the first letter.</param>
        /// <param name="tailLetterParser">A parser for trailing letters.</param>
        /// <returns>
        /// A parser for an identifier.
        /// </returns>
        public static Parser<string> Identifier(Parser<char> firstLetterParser, Parser<char> tailLetterParser)
        {
            return
                from firstLetter in firstLetterParser
                from tail in tailLetterParser.Many().Text()
                select firstLetter + tail;
        }
    }
}
