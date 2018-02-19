namespace Standard.Data.Markdown.Matchers
{
    internal sealed class AnyCharInRepeatMatcher : Matcher
    {
        private readonly char[] _ch;
        private readonly int _minOccur;
        private readonly int _maxOccur;

        public AnyCharInRepeatMatcher(char[] ch, int minOccur, int maxOccur)
        {
            _ch = ch;
            _minOccur = minOccur;
            _maxOccur = maxOccur;
        }

        public override int Match(MatchContent content)
        {
            var count = content.CountWhileAny(_ch, _maxOccur);
            if (count < _minOccur)
                return NotMatch;

            return count;
        }

        public override string ToString()
        {
            return "[" +
                EscapeText(string.Join(string.Empty, _ch)) +
                "]{" +
                _minOccur.ToString() +
                "," +
                (_maxOccur == int.MaxValue ? string.Empty : _maxOccur.ToString()) +
                "}";
        }
    }
}
