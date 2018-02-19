namespace Standard.Data.Markdown
{
    /// <summary>
    /// Null object.
    /// </summary>
    internal sealed class MarkdownNullTokenRewriter : IMarkdownTokenRewriter
    {
        public IMarkdownToken Rewrite(IMarkdownRewriteEngine engine, IMarkdownToken token)
        {
            return null;
        }
    }
}
