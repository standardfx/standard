namespace Standard.Data.Markdown
{
    /// <summary>
    /// Markdown rewritable (for object contains <see cref="IMarkdownToken"/>).
    /// </summary>
    /// <typeparam name="T">The type of implement this interface.</typeparam>
    public interface IMarkdownRewritable<out T>
    {
        /// <summary>
        /// Rewrite object with <see cref="IMarkdownRewriteEngine"/>
        /// </summary>
        /// <param name="rewriteEngine">The rewrite engine</param>
        /// <returns>The rewritten object.</returns>
        T Rewrite(IMarkdownRewriteEngine rewriteEngine);
    }
}
