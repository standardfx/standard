using System.Text.RegularExpressions;

namespace Standard.Data.Markdown
{
    public class MarkdownCommentInlineRule : IMarkdownRule
    {
        public virtual string Name => "Inline.Comment";

        public virtual Regex Comment => Regexes.Inline.Comment;

        public virtual IMarkdownToken TryMatch(IMarkdownParser parser, IMarkdownParsingContext context)
        {
            var match = Comment.Match(context.CurrentMarkdown);
            if (match.Length == 0)
                return null;

            var sourceInfo = context.Consume(match.Length);
            return new MarkdownRawToken(this, parser.Context, sourceInfo);
        }
    }
}
