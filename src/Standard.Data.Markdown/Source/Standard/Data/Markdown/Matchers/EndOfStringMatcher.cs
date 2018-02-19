namespace Standard.Data.Markdown.Matchers
{
    internal sealed class EndOfStringMatcher : Matcher
    {
        public override int Match(MatchContent content)
        {
            return content.EndOfString() ? 0 : NotMatch;
        }

        public override string ToString()
        {
            return "$";
        }
    }
}
