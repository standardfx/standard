using System;

namespace Standard.Data.Markdown.Matchers
{
    internal sealed class AnyCharNotInMatcher : Matcher, IRepeatable
    {
        private readonly char[] _ch;

        public AnyCharNotInMatcher(char[] ch)
        {
            _ch = ch;
        }

        public override int Match(MatchContent content)
        {
            if (content.EndOfString())
                return NotMatch;

            return Array.BinarySearch(_ch, content.GetCurrentChar()) >= 0 ? NotMatch : 1;
        }

        public Matcher Repeat(int minOccur, int maxOccur)
        {
            return new AnyCharNotInRepeatMatcher(_ch, minOccur, maxOccur);
        }

        public override string ToString()
        {
            return "[^" + EscapeText(string.Join(string.Empty, _ch)) + "]";
        }
    }
}
