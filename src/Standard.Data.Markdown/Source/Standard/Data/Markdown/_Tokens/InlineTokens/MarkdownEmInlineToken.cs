using System.Collections.Generic;
using System.Collections.Immutable;

namespace Standard.Data.Markdown
{
    public class MarkdownEmInlineToken : IMarkdownExpression, IMarkdownRewritable<MarkdownEmInlineToken>
    {
        public MarkdownEmInlineToken(IMarkdownRule rule, IMarkdownContext context, ImmutableArray<IMarkdownToken> content, SourceInfo sourceInfo)
        {
            Rule = rule;
            Context = context;
            Content = content;
            SourceInfo = sourceInfo;
        }

        public IMarkdownRule Rule { get; }

        public IMarkdownContext Context { get; }

        public ImmutableArray<IMarkdownToken> Content { get; }

        public SourceInfo SourceInfo { get; }

        public MarkdownEmInlineToken Rewrite(IMarkdownRewriteEngine rewriterEngine)
        {
            var tokens = rewriterEngine.Rewrite(Content);
            if (tokens == Content)
                return this;

            return new MarkdownEmInlineToken(Rule, Context, tokens, SourceInfo);
        }

        public IEnumerable<IMarkdownToken> GetChildren() => Content;
    }
}
