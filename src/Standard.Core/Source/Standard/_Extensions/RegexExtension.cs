using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Standard.Core;

namespace Standard
{
    public static class RegexExtension
    {
        // GroupValue vs GroupValues
        // violate Standard design guideline for plural form, but in line with regex Match/Matches.
        
        public static string GroupValue(this Regex regex, string matchExpr, string groupName)
        {
            if (regex == null)
                throw new ArgumentNullException(nameof(regex));

            if (string.IsNullOrEmpty(matchExpr))
                throw new ArgumentNullException(nameof(matchExpr));

            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentNullException(nameof(groupName));

            Match m = regex.Match(matchExpr);
            return m.Groups[groupName].Value;
        }

        public static string GroupValue(this Regex regex, string matchExpr, int groupIndex)
        {
            if (regex == null)
                throw new ArgumentNullException(nameof(regex));

            if (string.IsNullOrEmpty(matchExpr))
                throw new ArgumentNullException(nameof(matchExpr));

            if (groupIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(groupIndex));

            Match m = regex.Match(matchExpr);
            return m.Groups[groupIndex].Value;
        }

        public static List<string> GroupValues(this Regex regex, string matchExpr, string groupName)
        {
            if (regex == null)
                throw new ArgumentNullException(nameof(regex));

            if (string.IsNullOrEmpty(matchExpr))
                throw new ArgumentNullException(nameof(matchExpr));

            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentNullException(nameof(groupName));

            MatchCollection matches = regex.Matches(matchExpr);
            List<string> values = new List<string>();
            foreach (Match m in matches)
            {
                string extracted = m.Groups[groupName].Value;
                if (!StringUtility.IsNullOrWhiteSpace(extracted))
                    values.Add(extracted);
            }

            return values;
        }

        public static List<string> GroupValues(this Regex regex, string matchExpr, int groupIndex)
        {
            if (regex == null)
                throw new ArgumentNullException(nameof(regex));

            if (string.IsNullOrEmpty(matchExpr))
                throw new ArgumentNullException(nameof(matchExpr));

            if (groupIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(groupIndex));
                
            MatchCollection matches = regex.Matches(matchExpr);
            List<string> values = new List<string>();
            foreach (Match m in matches)
            {
                string extracted = m.Groups[groupIndex].Value;
                if (!StringUtility.IsNullOrWhiteSpace(extracted))
                    values.Add(extracted);
            }

            return values;
        }
    }
}