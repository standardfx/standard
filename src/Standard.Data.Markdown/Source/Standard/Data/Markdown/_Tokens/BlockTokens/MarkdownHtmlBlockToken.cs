using System.Collections.Generic;

namespace Standard.Data.Markdown
{
    public class MarkdownHtmlBlockToken : IMarkdownExpression, IMarkdownRewritable<MarkdownHtmlBlockToken>
    {
        public MarkdownHtmlBlockToken(IMarkdownRule rule, IMarkdownContext context, InlineContent content, SourceInfo sourceInfo)
        {
            Rule = rule;
            Context = context;
            Content = content;
            SourceInfo = sourceInfo;
        }

        public IMarkdownRule Rule { get; }

        public IMarkdownContext Context { get; }

        public InlineContent Content { get; }

        public SourceInfo SourceInfo { get; }

        public MarkdownHtmlBlockToken Rewrite(IMarkdownRewriteEngine rewriterEngine)
        {
            var c = Content.Rewrite(rewriterEngine);
            if (c == Content)
                return this;

            return new MarkdownHtmlBlockToken(Rule, Context, c, SourceInfo);
        }

        public IEnumerable<IMarkdownToken> GetChildren() => Content.Tokens;
    }
}
