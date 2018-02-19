using System.Text.RegularExpressions;

namespace Standard.Data.Markdown
{
    public class MarkdownPreElementInlineRule : IMarkdownRule
    {
        public virtual string Name => "Inline.CodeElement";

        public virtual Regex PreElement => Regexes.Inline.PreElement;

        public virtual IMarkdownToken TryMatch(IMarkdownParser parser, IMarkdownParsingContext context)
        {
            var match = PreElement.Match(context.CurrentMarkdown);
            if (match.Length == 0)
                return null;

            var sourceInfo = context.Consume(match.Length);
            return new MarkdownRawToken(this, parser.Context, sourceInfo);
        }
    }
}
