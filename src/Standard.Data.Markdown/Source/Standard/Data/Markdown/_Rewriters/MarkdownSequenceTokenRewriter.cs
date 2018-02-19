using System.Collections.Immutable;

namespace Standard.Data.Markdown
{
    internal sealed class MarkdownSequenceTokenRewriter : IMarkdownTokenRewriter, IInitializable
    {
        public MarkdownSequenceTokenRewriter(ImmutableArray<IMarkdownTokenRewriter> inner)
        {
            Inner = inner;
        }

        public ImmutableArray<IMarkdownTokenRewriter> Inner { get; }

        public void Initialize(IMarkdownRewriteEngine rewriteEngine)
        {
            foreach (var item in Inner)
            {
                (item as IInitializable)?.Initialize(rewriteEngine);
            }
        }

        public IMarkdownToken Rewrite(IMarkdownRewriteEngine engine, IMarkdownToken token)
        {
            IMarkdownToken newToken = token;
            for (int index = 0; index < Inner.Length; index++)
            {
                newToken = Inner[index].Rewrite(engine, newToken) ?? newToken;
            }
            return newToken;
        }
    }
}
