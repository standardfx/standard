using System;

namespace Standard.Data.Markdown
{
    internal sealed class MarkdownLambdaTokenRewriter<TEngine, TToken> : IMarkdownTokenRewriter
        where TEngine : class, IMarkdownRewriteEngine
        where TToken : class, IMarkdownToken
    {
        public MarkdownLambdaTokenRewriter(Func<TEngine, TToken, IMarkdownToken> rewriteFunc)
        {
            RewriteFunc = rewriteFunc;
        }

        public Func<TEngine, TToken, IMarkdownToken> RewriteFunc { get; }

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
