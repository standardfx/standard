using System;
using System.Collections.Immutable;

namespace Standard.Data.Markdown
{
    /// <summary>
    /// Markdown rewrite engine.
    /// </summary>
    public interface IMarkdownRewriteEngine
    {
        /// <summary>
        /// Get markdown engine.
        /// </summary>
        IMarkdownEngine Engine { get; }

        /// <summary>
        /// Rewrite markdown tokens.
        /// </summary>
        /// <param name="tokens">Source markdown tokens.</param>
        /// <returns>Rewritten markdown tokens.</returns>
        ImmutableArray<IMarkdownToken> Rewrite(ImmutableArray<IMarkdownToken> tokens);

        ImmutableArray<IMarkdownToken> GetParents();

        bool HasVariable(string name);

        object GetVariable(string name);

        void SetVariable(string name, object value);

        void RemoveVariable(string name);

        bool HasPostProcess(string name);

        void SetPostProcess(string name, Action<IMarkdownRewriteEngine> action);

        void RemovePostProcess(string name);

        void Initialize();

        void Complete();
    }
}
