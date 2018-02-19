using System.Collections.Immutable;

namespace Standard.Data.Markdown
{
    internal sealed class MarkdownCompositeTokenRewriter : IMarkdownTokenRewriter, IInitializable
    {
        public MarkdownCompositeTokenRewriter(ImmutableList<IMarkdownTokenRewriter> rewriters)
        {
            Rewriters = rewriters;
        }

        public ImmutableList<IMarkdownTokenRewriter> Rewriters { get; }

        public void Initialize(IMarkdownRewriteEngine rewriteEngine)
        {
            foreach (var item in Rewriters)
            {
                (item as IInitializable)?.Initialize(rewriteEngine);
            }
        }

        public IMarkdownToken Rewrite(IMarkdownRewriteEngine engine, IMarkdownToken token)
        {
            if (Rewriters.Count > 0)
            {
                foreach (var rewriter in Rewriters)
                {
                    var newToken = rewriter.Rewrite(engine, token);
                    if (newToken != null)
                        return newToken;
                }
            }
            return null;
        }
    }
}
