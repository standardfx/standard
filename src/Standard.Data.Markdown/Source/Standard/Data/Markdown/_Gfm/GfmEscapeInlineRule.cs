using System.Text.RegularExpressions;

namespace Standard.Data.Markdown
{
    public class GfmEscapeInlineRule : MarkdownEscapeInlineRule
    {
        public override Regex Escape => Regexes.Inline.Gfm.Escape;
    }
}
