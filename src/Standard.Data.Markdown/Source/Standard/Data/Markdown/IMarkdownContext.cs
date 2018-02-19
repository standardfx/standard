using System.Collections.Immutable;

namespace Standard.Data.Markdown
{
    /// <summary>
    /// The context for markdown parser.
    /// It should be immutable.
    /// </summary>
    public interface IMarkdownContext
    {
        /// <summary>
        /// Get the rule set for current context.
        /// </summary>
        ImmutableList<IMarkdownRule> Rules { get; }

        /// <summary>
        /// Get the variables.
        /// </summary>
        ImmutableDictionary<string, object> Variables { get; }

        /// <summary>
        /// Create a new context with different variables.
        /// </summary>
        /// <param name="variables">The new variables.</param>
        /// <returns>a new instance of <see cref="IMarkdownContext"/></returns>
        IMarkdownContext CreateContext(ImmutableDictionary<string, object> variables);
    }
}
