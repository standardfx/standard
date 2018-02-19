using System.Collections.Generic;
using System.Collections.Immutable;

namespace Standard.Data.Markdown
{
    /// <summary>
    /// Markdown parser.
    /// </summary>
    public interface IMarkdownParser
    {
        /// <summary>
        /// Get the current markdown context.
        /// </summary>
        IMarkdownContext Context { get; }

        /// <summary>
        /// Get the No. links.
        /// </summary>
        Dictionary<string, LinkObj> Links { get; }

        /// <summary>
        /// Get the <see cref="Options"/>.
        /// </summary>
        Options Options { get; }

        /// <summary>
        /// Switch the markdown context.
        /// </summary>
        /// <param name="context">New context.</param>
        /// <returns>The old context.</returns>
        IMarkdownContext SwitchContext(IMarkdownContext context);

        /// <summary>
        /// Tokenize the markdown text.
        /// </summary>
        /// <param name="sourceInfo">The markdown source.</param>
        /// <returns>A list of <see cref="IMarkdownToken"/>.</returns>
        ImmutableArray<IMarkdownToken> Tokenize(SourceInfo sourceInfo);
    }
}
