using System;

namespace Standard.Data.Markdown
{
    internal sealed class MarkdownInitializableLambdaTokenRewriter<TEngine, TToken>
        : IMarkdownTokenRewriter, IInitializable
        where TEngine : class, IMarkdownRewriteEngine
        where TToken : class, IMarkdownToken
    {
        public MarkdownInitializableLambdaTokenRewriter(
            Func<TEngine, TToken, IMarkdownToken> rewriteFunc,
            Action<TEngine> initializer)
        {
            RewriteFunc = rewriteFunc;
            Initializer = initializer;
        }

        public Func<TEngine, TToken, IMarkdownToken> RewriteFunc { get; }

        public Action<TEngine> Initializer { get; }

        public void Initialize(IMarkdownRewriteEngine rewriteEngine)
        {
            var tengine = rewriteEngine as TEngine;
            if (tengine != null)
                Initializer(tengine);
        }

        public IMarkdownToken Rewrite(IMarkdownRewriteEngine engine, IMarkdownToken token)
        {
            var tengine = engine as TEngine;
            var ttoken = token as TToken;
            if (tengine != null && ttoken != null)
                return RewriteFunc(tengine, ttoken);

            return null;
        }
    }
}
