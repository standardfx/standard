using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Standard.Data.Markdown
{
    public static class MarkdownTokenRewriterFactory
    {
        public static readonly IMarkdownTokenRewriter Null = new MarkdownNullTokenRewriter();

        public static IMarkdownTokenRewriter FromLambda<TEngine, TToken>(
            Func<TEngine, TToken, IMarkdownToken> rewriteFunc)
            where TEngine : class, IMarkdownRewriteEngine
            where TToken : class, IMarkdownToken
        {
            if (rewriteFunc == null)
                throw new ArgumentNullException(nameof(rewriteFunc));

            return new MarkdownLambdaTokenRewriter<TEngine, TToken>(rewriteFunc);
        }

        public static IMarkdownTokenRewriter FromLambda<TEngine, TToken>(
            Func<TEngine, TToken, IMarkdownToken> rewriteFunc,
            Action<TEngine> initializer)
            where TEngine : class, IMarkdownRewriteEngine
            where TToken : class, IMarkdownToken
        {
            if (rewriteFunc == null)
                throw new ArgumentNullException(nameof(rewriteFunc));

			if (initializer == null)
                return new MarkdownLambdaTokenRewriter<TEngine, TToken>(rewriteFunc);

            return new MarkdownInitializableLambdaTokenRewriter<TEngine, TToken>(rewriteFunc, initializer);
        }

        public static IMarkdownTokenRewriter FromValidators(IEnumerable<IMarkdownTokenValidator> validators)
        {
            if (validators == null)
                throw new ArgumentNullException(nameof(validators));

            return new MarkdownTokenValidatorAdapter(validators);
        }

        public static IMarkdownTokenRewriter FromValidators(params IMarkdownTokenValidator[] validators)
        {
            if (validators == null)
                throw new ArgumentNullException(nameof(validators));

            return new MarkdownTokenValidatorAdapter(validators);
        }

        public static IMarkdownTokenRewriter FromValidators(string scopeName, IEnumerable<IMarkdownTokenValidator> validators)
        {
            if (validators == null)
                throw new ArgumentNullException(nameof(validators));

            return new MarkdownTokenValidatorAdapter(validators);
        }

        public static IMarkdownTokenRewriter Composite(params IMarkdownTokenRewriter[] rewriters)
        {
            return Composite((IEnumerable<IMarkdownTokenRewriter>)rewriters);
        }

        public static IMarkdownTokenRewriter Composite(IEnumerable<IMarkdownTokenRewriter> rewriters)
        {
            if (rewriters == null)
                throw new ArgumentNullException(nameof(rewriters));

            return new MarkdownCompositeTokenRewriter(rewriters.ToImmutableList());
        }

        public static IMarkdownTokenRewriter Loop(IMarkdownTokenRewriter rewriter, int maxLoopCount)
        {
            if (rewriter == null)
                throw new ArgumentNullException(nameof(rewriter));

            if (maxLoopCount <= 0)
                throw new ArgumentOutOfRangeException(RS.ExpectMoreThanZero, nameof(maxLoopCount));

            return new MarkdownLoopTokenRewriter(rewriter, maxLoopCount);
        }

        public static IMarkdownTokenRewriter Sequence(params IMarkdownTokenRewriter[] rewriters)
        {
            if (rewriters == null)
                throw new ArgumentNullException(nameof(rewriters));

            if (rewriters.Length == 0)
                throw new ArgumentException(RS.ExpectNonEmptyArray, nameof(rewriters));

            for (int i = 0; i < rewriters.Length; i++)
            {
                if (rewriters[i] == null)
                    throw new ArgumentException(RS.ExpectNoNullMember, nameof(rewriters));
            }
            return new MarkdownSequenceTokenRewriter(rewriters.ToImmutableArray());
        }
    }
}
