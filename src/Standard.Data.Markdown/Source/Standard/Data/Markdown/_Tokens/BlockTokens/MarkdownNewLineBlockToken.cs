namespace Standard.Data.Markdown
{
    public class MarkdownNewLineBlockToken : IMarkdownToken
    {
        public MarkdownNewLineBlockToken(IMarkdownRule rule, IMarkdownContext context, SourceInfo sourceInfo)
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
