namespace Standard.Data.Markdown.Matchers
{
    internal sealed class CharRepeatMatcher : Matcher
    {
        private readonly char _ch;
        private readonly int _minOccur;
        private readonly int _maxOccur;

        public CharRepeatMatcher(char ch, int minOccur, int maxOccur)
        {
            _ch = ch;
            _minOccur = minOccur;
            _maxOccur = maxOccur;
        }

        public override int Match(MatchContent content)
        {
            var count = content.CountWhile(_ch, _maxOccur);
            if (count < _minOccur)
                return NotMatch;

            return count;
        }

        public override string ToString()
        {
            return EscapeText(_ch.ToString()) +
                "{" +
                _minOccur.ToString() +
                "," +
                (_maxOccur == int.MaxValue ? string.Empty : _maxOccur.ToString()) +
                "}";
        }
    }
}
