using System;
using System.Collections.Generic;

namespace Standard.Data.Markdown
{
    public static class MarkdownParserExtensions
    {
        public static IMarkdownContext SwitchContext(this IMarkdownParser parser, string variableKey, object value)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

			if (variableKey == null)
                throw new ArgumentNullException(nameof(variableKey));

            return parser.SwitchContext(
                parser.Context.CreateContext(
                    parser.Context.Variables.SetItem(variableKey, value)));
        }

        public static IMarkdownContext SwitchContext(this IMarkdownParser parser, IReadOnlyDictionary<string, object> variables)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            if (variables == null)
                throw new ArgumentNullException(nameof(variables));

            var builder = parser.Context.Variables.ToBuilder();
            foreach (var pair in variables)
            {
                builder[pair.Key] = pair.Value;
            }
            return parser.SwitchContext(
                parser.Context.CreateContext(
                    builder.ToImmutable()));
        }

        public static InlineContent TokenizeInline(this IMarkdownParser parser, SourceInfo sourceInfo)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            var context = parser.Context as MarkdownBlockContext;
            if (context == null)
                throw new InvalidOperationException(string.Format(RS.InvalidToken, nameof(parser), nameof(parser.Context), parser.Context.GetType().FullName));

            var c = parser.SwitchContext(context.GetInlineContext());
            var tokens = parser.Tokenize(sourceInfo);
            parser.SwitchContext(c);
            return new InlineContent(tokens);
        }
    }
}
