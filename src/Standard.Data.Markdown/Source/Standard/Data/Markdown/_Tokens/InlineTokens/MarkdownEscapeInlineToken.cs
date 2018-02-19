namespace Standard.Data.Markdown
{
    public class MarkdownEscapeInlineToken : IMarkdownToken
    {
        public MarkdownEscapeInlineToken(IMarkdownRule rule, IMarkdownContext context, string content, SourceInfo sourceInfo)
        {
            Rule = rule;
            Context = context;
            Content = content;
            SourceInfo = sourceInfo;
        }

        public IMarkdownRule Rule { get; }

        public IMarkdownContext Context { get; }

        public string Content { get; }

        public SourceInfo SourceInfo { get; }
    }
}
