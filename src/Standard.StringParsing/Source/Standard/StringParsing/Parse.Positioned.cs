namespace Standard.StringParsing
{
    partial class Parse
    {
        /// <summary>
        /// Creates a parser that will set the position to the position-aware type 
        /// on succsssful match.
        /// </summary>
        /// <typeparam name="T">The result type of the specified parser.</typeparam>
        /// <param name="parser">The parser to wrap.</param>
        /// <returns>
        /// An position aware version of the specified parser.
        /// </returns>
        public static Parser<T> Positioned<T>(this Parser<T> parser) where T : IPositionAware<T>
        {
            return i =>
            {
                IResult<T> r = parser(i);

                if (r.WasSuccessful)
                    return Result.Success(r.Value.SetPosition(Position.FromInput(i), r.Remainder.Position - i.Position), r.Remainder);

                return r;
            };
        }
    }
}
