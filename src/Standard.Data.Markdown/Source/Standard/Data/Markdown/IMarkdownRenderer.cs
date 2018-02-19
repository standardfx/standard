using System.Collections.Generic;

namespace Standard.Data.Markdown
{
    /// <summary>
    /// Markdown renderer.
    /// </summary>
    public interface IMarkdownRenderer
    {
        /// <summary>
        /// Get the markdown engine.
        /// </summary>
        IMarkdownEngine Engine { get; }

        /// <summary>
        /// Get the No. links.
        /// </summary>
        Dictionary<string, LinkObj> Links { get; }
        
        /// <summary>
        /// Get the <see cref="Options"/>.
        /// </summary>
        Options Options { get; }

        /// <summary>
        /// Render a token.
        /// </summary>
        /// <param name="token">The token to render.</param>
        /// <returns>The text.</returns>
        StringBuffer Render(IMarkdownToken token);
    }
}
