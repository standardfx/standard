namespace Standard.Data.Markdown
{
    public class MarkdownTextToken : IMarkdownToken
    {
        public MarkdownTextToken(IMarkdownRule rule, IMarkdownContext context, string content, SourceInfo sourceInfo)
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
