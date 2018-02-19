namespace Standard.Data.Markdown
{
    public class MarkdownCodeBlockToken : IMarkdownToken
    {
        public MarkdownCodeBlockToken(IMarkdownRule rule, IMarkdownContext context, string code, string lang, SourceInfo sourceInfo)
        {
            Rule = rule;
            Context = context;
            Code = code;
            Lang = lang;
            SourceInfo = sourceInfo;
        }

        public IMarkdownRule Rule { get; }

        public IMarkdownContext Context { get; }

        public string Code { get; }

        public string Lang { get; }

        public SourceInfo SourceInfo { get; }
    }
}
