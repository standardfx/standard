namespace Standard.Data.Markdown.Matchers
{
    internal sealed class TestMatcher : Matcher
    {
        private readonly Matcher[] _inner;
        private readonly bool _isNegative;

        public TestMatcher(Matcher[] inner, bool isNegative)
        {
            _inner = inner;
            _isNegative = isNegative;
        }

        public override int Match(MatchContent content)
        {
            foreach (var m in _inner)
            {
                if (_isNegative ^ (m.Match(content) == NotMatch))
                    return NotMatch;
            }
            return 0;
        }

        public override string ToString()
        {
            return (_isNegative ? "(?!" : "(?=") + string.Join<Matcher>("|", _inner) + ")";
        }
    }
}
