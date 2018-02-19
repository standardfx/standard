using System.Text.RegularExpressions;

namespace Standard.Data.Markdown
{
    public class MarkdownStrongInlineRule : IMarkdownRule
    {
        public virtual string Name => "Inline.Strong";

        public virtual Regex Strong => Regexes.Inline.Strong;

        public virtual IMarkdownToken TryMatch(IMarkdownParser parser, IMarkdownParsingContext context)
        {
            var match = Strong.Match(context.CurrentMarkdown);
            if (match.Length == 0)
                return null;

            var sourceInfo = context.Consume(match.Length);
            return new MarkdownStrongInlineToken(this, parser.Context, parser.Tokenize(sourceInfo.Copy(match.NotEmpty(2, 1))), sourceInfo);
        }
    }
}
