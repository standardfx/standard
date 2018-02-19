namespace Standard.Data.Markdown.Matchers
{
    internal sealed class CharMatcher : Matcher, IRepeatable
    {
        private readonly char _ch;

        public CharMatcher(char ch)
        {
            _ch = ch;
        }

        public override int Match(MatchContent content)
        {
            if (content.EndOfString())
                return NotMatch;

            return content.GetCurrentChar() == _ch ? 1 : NotMatch;
        }

        public Matcher Repeat(int minOccur, int maxOccur)
        {
            return new CharRepeatMatcher(_ch, minOccur, maxOccur);
        }

        public override string ToString()
        {
            return EscapeText(_ch.ToString());
        }
    }
}
