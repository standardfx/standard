using System.Collections.Immutable;

namespace Standard.Data.Markdown
{
    public class InlineContent : IMarkdownRewritable<InlineContent>
    {
        public InlineContent(ImmutableArray<IMarkdownToken> tokens)
        {
            Tokens = tokens;
        }

        public ImmutableArray<IMarkdownToken> Tokens { get; }

        public InlineContent Rewrite(IMarkdownRewriteEngine rewriterEngine)
        {
            var tokens = rewriterEngine.Rewrite(Tokens);
            if (tokens == Tokens)
                return this;

            return new InlineContent(tokens);
        }
    }
}
