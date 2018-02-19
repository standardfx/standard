namespace Standard.Data.Parsing
{
    partial class Parse
    {
        /// <summary>
        /// Construct a parser that will set the position to the position-aware
        /// T on succsessful match.
        /// </summary>
        public static Parser<T> Positioned<T>(this Parser<T> parser) where T : IPositionAware<T>
        {
            return i =>
            {
                IResult<T> r = parser(i);

                if (r.WasSuccessful)
                    return Result.Success(r.Value.SetPos(Position.FromInput(i), r.Remainder.Position - i.Position), r.Remainder);

                return r;
            };
        }
    }
}
