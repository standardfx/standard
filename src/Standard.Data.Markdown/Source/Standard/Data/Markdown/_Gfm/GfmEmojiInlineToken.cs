namespace Standard.Data.Markdown
{
    public class GfmEmojiInlineToken : IMarkdownToken
    {
        public GfmEmojiInlineToken(IMarkdownRule rule, IMarkdownContext context, string shortCode, string emoji, SourceInfo sourceInfo)
        {
            Rule = rule;
            Context = context;
            ShortCode = shortCode;
            Emoji = emoji;
            SourceInfo = sourceInfo;
        }

        public IMarkdownRule Rule { get; }

        public IMarkdownContext Context { get; }

        public string ShortCode { get; }

        public string Emoji { get; }

        public SourceInfo SourceInfo { get; }
    }
}
