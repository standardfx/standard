using System.Collections.Generic;
using System.Collections.Immutable;

namespace Standard.Data.Markdown
{
    public class MarkdownBlockquoteBlockToken : IMarkdownExpression, IMarkdownRewritable<MarkdownBlockquoteBlockToken>
    {
        public MarkdownBlockquoteBlockToken(IMarkdownRule rule, IMarkdownContext context, ImmutableArray<IMarkdownToken> tokens, SourceInfo sourceInfo)
        {
            Rule = rule;
            Context = context;
            Tokens = tokens;
            SourceInfo = sourceInfo;
        }

        public IMarkdownRule Rule { get; }

        public IMarkdownContext Context { get; }

        public ImmutableArray<IMarkdownToken> Tokens { get; }

        public SourceInfo SourceInfo { get; }

        public MarkdownBlockquoteBlockToken Rewrite(IMarkdownRewriteEngine rewriterEngine)
        {
            var tokens = rewriterEngine.Rewrite(Tokens);
            if (tokens == Tokens)
                return this;

            return new MarkdownBlockquoteBlockToken(Rule, Context, tokens, SourceInfo);
        }

        public IEnumerable<IMarkdownToken> GetChildren() => Tokens;
    }
}
