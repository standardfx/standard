using System.Text.RegularExpressions;

namespace Standard.Data.Markdown
{
    public class MarkdownEmInlineRule : IMarkdownRule
    {
        public virtual string Name => "Inline.Em";

        public virtual Regex Em => Regexes.Inline.Em;

        public virtual IMarkdownToken TryMatch(IMarkdownParser parser, IMarkdownParsingContext context)
        {
            var match = Em.Match(context.CurrentMarkdown);
            if (match.Length == 0)
                return null;

            var sourceInfo = context.Consume(match.Length);
            return new MarkdownEmInlineToken(this, parser.Context, parser.Tokenize(sourceInfo.Copy(match.NotEmpty(2, 1))), sourceInfo);
        }
    }
}
