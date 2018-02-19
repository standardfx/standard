namespace Standard.Data.Markdown
{
    public class MarkdownRawToken : IMarkdownToken
    {
        public MarkdownRawToken(IMarkdownRule rule, IMarkdownContext context, SourceInfo sourceInfo)
        {
            Rule = rule;
            Context = context;
            SourceInfo = sourceInfo;
        }

        public IMarkdownRule Rule { get; }

        public IMarkdownContext Context { get; }

        public SourceInfo SourceInfo { get; }
    }
}
