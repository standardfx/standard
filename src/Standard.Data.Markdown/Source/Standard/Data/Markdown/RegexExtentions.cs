using System.Text.RegularExpressions;

namespace Standard.Data.Markdown
{
    internal static class RegexExtentions
    {
        public static string NotEmpty(this Match match, int index1, int index2)
        {
            if (match.Groups.Count > index1 && !string.IsNullOrEmpty(match.Groups[index1].Value))
                return match.Groups[index1].Value;

            return match.Groups[index2].Value;
        }
    }
}
