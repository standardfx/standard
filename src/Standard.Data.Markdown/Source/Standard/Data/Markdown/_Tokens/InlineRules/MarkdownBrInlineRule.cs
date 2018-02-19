using System.Text.RegularExpressions;

namespace Standard.Data.Markdown
{
    public class MarkdownBrInlineRule : IMarkdownRule
    {
        public virtual string Name => "Inline.Br";

        public virtual Regex Br => Regexes.Inline.Br;

        public virtual IMarkdownToken TryMatch(IMarkdownParser parser, IMarkdownParsingContext context)
        {
            var match = Br.Match(context.CurrentMarkdown);
            if (match.Length == 0)
                return null;

            var sourceInfo = context.Consume(match.Length);
            return new MarkdownBrInlineToken(this, parser.Context, sourceInfo);
        }
    }
}
