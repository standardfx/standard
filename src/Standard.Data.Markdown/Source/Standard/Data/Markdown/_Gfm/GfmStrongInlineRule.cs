using System.Text.RegularExpressions;

namespace Standard.Data.Markdown
{
    public class GfmStrongInlineRule : MarkdownStrongInlineRule
    {
        public override string Name => "Inline.Gfm.Strong";

        public override Regex Strong => Regexes.Inline.Gfm.Strong;
    }
}
