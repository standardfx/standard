using System;
using System.Text.RegularExpressions;
using Standard.Data.Markdown.Matchers;

namespace Standard.Data.Markdown
{
    public class MarkdownNewLineBlockRule : IMarkdownRule
    {
        private static readonly Matcher _NewLineMatcher = Matcher.NewLine.RepeatAtLeast(1);

        public virtual string Name => "NewLine";

        [Obsolete("Please use NewLineMatcher.")]
        public virtual Regex Newline => Regexes.Block.Newline;

        public virtual Matcher NewLineMatcher => _NewLineMatcher;

        public virtual IMarkdownToken TryMatch(IMarkdownParser parser, IMarkdownParsingContext context)
        {
            if (Newline != Regexes.Block.Newline)
                return TryMatchOld(parser, context);

            var match = context.Match(NewLineMatcher);
            if (match?.Length > 0)
                return new MarkdownNewLineBlockToken(this, parser.Context, context.Consume(match.Length));

            return null;
        }

        private IMarkdownToken TryMatchOld(IMarkdownParser parser, IMarkdownParsingContext context)
        {
            var match = Newline.Match(context.CurrentMarkdown);
            if (match.Length == 0)
                return null;

            var sourceInfo = context.Consume(match.Length);
            return new MarkdownNewLineBlockToken(this, parser.Context, sourceInfo);
        }
    }
}
