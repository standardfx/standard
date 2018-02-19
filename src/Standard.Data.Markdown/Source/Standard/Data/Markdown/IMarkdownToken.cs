namespace Standard.Data.Markdown
{
    /// <summary>
    /// The token for markdown.
    /// It should be immutable.
    /// </summary>
    public interface IMarkdownToken
    {
        /// <summary>
        /// The rule created this token.
        /// </summary>
        IMarkdownRule Rule { get; }

        /// <summary>
        /// The context when created this token.
        /// </summary>
        IMarkdownContext Context { get; }

        /// <summary>
        /// The source info of this token.
        /// </summary>
        SourceInfo SourceInfo { get; }
    }
}
