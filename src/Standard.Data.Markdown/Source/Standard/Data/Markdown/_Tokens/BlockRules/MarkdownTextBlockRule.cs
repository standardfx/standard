using System;
using System.Text.RegularExpressions;
using Standard.Data.Markdown.Matchers;

namespace Standard.Data.Markdown
{
    public class MarkdownTextBlockRule : IMarkdownRule
    {
        // @"^[^\n]+\n?"
        private static readonly Matcher _TextMatcher =
            Matcher.AnyStringInSingleLine + Matcher.NewLine.Maybe();

        public virtual string Name => "Text";

        [Obsolete("Please use LHeadingMatcher.")]
        public virtual Regex Text => Regexes.Block.Text;

        public virtual Matcher TextMatcher => _TextMatcher;

        public virtual IMarkdownToken TryMatch(IMarkdownParser parser, IMarkdownParsingContext context)
        {
            if (Text != Regexes.Block.Text)
                return TryMatchOld(parser, context);

            var match = context.Match(TextMatcher);
            if (match?.Length > 0)
            {
                var sourceInfo = context.Consume(match.Length);
                return new MarkdownTextToken(this, parser.Context, sourceInfo.Markdown, sourceInfo);
            }
            return null;
        }

        public virtual IMarkdownToken TryMatchOld(IMarkdownParser parser, IMarkdownParsingContext context)
        {
            var match = Text.Match(context.CurrentMarkdown);
            if (match.Length == 0)
                return null;

            var sourceInfo = context.Consume(match.Length);
            return new MarkdownTextToken(this, parser.Context, match.Value, sourceInfo);
        }
    }
}
