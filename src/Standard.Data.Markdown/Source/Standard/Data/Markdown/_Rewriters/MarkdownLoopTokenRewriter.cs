using System;

namespace Standard.Data.Markdown
{
    internal sealed class MarkdownLoopTokenRewriter : IMarkdownTokenRewriter, IInitializable
    {
        public MarkdownLoopTokenRewriter(IMarkdownTokenRewriter inner, int maxLoopCount)
        {
            Inner = inner;
            MaxLoopCount = maxLoopCount;
        }

        public IMarkdownTokenRewriter Inner { get; }

        public int MaxLoopCount { get; }

        public void Initialize(IMarkdownRewriteEngine rewriteEngine)
        {
            (Inner as IInitializable)?.Initialize(rewriteEngine);
        }

        public IMarkdownToken Rewrite(IMarkdownRewriteEngine engine, IMarkdownToken token)
        {
            IMarkdownToken lastToken;
            IMarkdownToken newToken = token;
            for (int loopCount = 0; loopCount < MaxLoopCount; loopCount++)
            {
                lastToken = newToken;
                newToken = Inner.Rewrite(engine, lastToken);
                if (newToken == null)
                    return lastToken;
            }
            throw new InvalidOperationException(RS.TooManyLoops);
        }
    }
}
