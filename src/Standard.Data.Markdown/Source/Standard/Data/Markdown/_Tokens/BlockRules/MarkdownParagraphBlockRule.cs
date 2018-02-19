using System.Text.RegularExpressions;

namespace Standard.Data.Markdown
{
    public class MarkdownParagraphBlockRule : IMarkdownRule
    {
        public static readonly MarkdownParagraphBlockRule Instance = new MarkdownParagraphBlockRule();

        private MarkdownParagraphBlockRule() { }

        public string Name => "Paragraph";

        public IMarkdownToken TryMatch(IMarkdownParser parser, IMarkdownParsingContext context)
        {
            return null;
        }
    }
}
