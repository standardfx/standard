namespace Standard.Data.Markdown.Matchers
{
    internal sealed class ReverseMatcher : Matcher
    {
        private readonly Matcher _inner;

        public ReverseMatcher(Matcher inner)
        {
            _inner = inner;
        }

        public override int Match(MatchContent content)
        {
            return _inner.Match(content.Reverse());
        }

        public override string ToString()
        {
            return "(Reverse:" + _inner + ")";
        }
    }
}
