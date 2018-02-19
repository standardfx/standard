using Standard.Data.Markdown.Matchers;

namespace Standard.Data.Markdown
{
    public interface IMarkdownParsingContext
    {
        string Markdown { get; }
        string CurrentMarkdown { get; }
        int LineNumber { get; }
        string File { get; }
        bool IsInParagraph { get; set; }

        SourceInfo Consume(int charCount);
        MatchResult Match(Matcher matcher);
    }
}
