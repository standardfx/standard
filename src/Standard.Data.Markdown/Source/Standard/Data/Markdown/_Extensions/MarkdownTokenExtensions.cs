using System;
using System.Collections.Generic;
using System.Linq;

namespace Standard.Data.Markdown
{
    public static class MarkdownTokenExtensions
    {
		private static IEnumerable<IMarkdownToken> DescendantsCore(IMarkdownToken token) =>
			from child in ChildrenCore(token)
			from item in new[] { child }.Concat(DescendantsCore(child))
			select item;

		private static IEnumerable<IMarkdownToken> ChildrenCore(IMarkdownToken token) =>
			(token as IMarkdownExpression)?.GetChildren() ?? Enumerable.Empty<IMarkdownToken>();

		public static IEnumerable<IMarkdownToken> Children(this IMarkdownToken token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            return ChildrenCore(token);
        }

        public static IEnumerable<T> Children<T>(this IMarkdownToken token)
            where T : IMarkdownToken
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            return ChildrenCore(token).OfType<T>();
        }

        public static IEnumerable<IMarkdownToken> Descendants(this IMarkdownToken token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            return DescendantsCore(token);
        }

        public static IEnumerable<T> Descendants<T>(this IMarkdownToken token)
            where T : IMarkdownToken
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            return DescendantsCore(token).OfType<T>();
        }

        public static IEnumerable<IMarkdownToken> BlockTokens(this IEnumerable<IMarkdownToken> tokens)
        {
            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));

            return from token in tokens
               where token.Context is MarkdownBlockContext
               select token;
        }

        public static IEnumerable<IMarkdownToken> InlineTokens(this IEnumerable<IMarkdownToken> tokens)
        {
            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));

            return from token in tokens
               where token.Context is MarkdownInlineContext
               select token;
        }
    }
}
