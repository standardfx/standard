using System.Text.RegularExpressions;

namespace Standard.Data.Markdown
{
    public class MarkdownEscapeInlineRule : IMarkdownRule
    {
        public virtual string Name => "Inline.Escape";

        public virtual Regex Escape => Regexes.Inline.Escape;

        public virtual IMarkdownToken TryMatch(IMarkdownParser parser, IMarkdownParsingContext context)
        {
            var match = Escape.Match(context.CurrentMarkdown);
            if (match.Length == 0)
                return null;

            var sourceInfo = context.Consume(match.Length);
            return new MarkdownEscapeInlineToken(this, parser.Context, match.Groups[1].Value, sourceInfo);
        }
    }
}
