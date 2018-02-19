using System.Text.RegularExpressions;

namespace Standard.Data.Markdown
{
    public class GfmEmInlineRule : MarkdownEmInlineRule
    {
        public override string Name => "Inline.Gfm.Em";

        public override Regex Em => Regexes.Inline.Gfm.Em;
    }
}
