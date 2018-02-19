namespace Standard.Data.Markdown.Matchers
{
    internal sealed class AnyCharMatcher : Matcher
    {
        public AnyCharMatcher() { }

        public override int Match(MatchContent content)
        {
            return content.EndOfString() ? NotMatch : 1;
        }

        public override string ToString()
        {
            return ".";
        }
    }
}
