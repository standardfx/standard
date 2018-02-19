using System.Text.RegularExpressions;

namespace Standard.Data.Markdown
{
    public class GfmTextInlineRule : MarkdownTextInlineRule
    {
        public override Regex Text => Regexes.Inline.Gfm.Text;
    }
}
