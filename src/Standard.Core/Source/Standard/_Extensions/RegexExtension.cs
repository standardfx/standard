using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Standard.Core;

namespace Standard
{
    /// <summary>
    /// Common extensions for the <see cref="Regex"/> class.
    /// </summary>
    public static class RegexExtension
    {
        // GroupValue vs GroupValues
        // violate Standard design guideline for plural form, but in line with regex Match/Matches.
        
        /// <summary>
        /// Returns the first group that matches the regular expression specified.
        /// </summary>
        /// <param name="regex">The given <see cref="Regex"/> instance.</param>
        /// <param name="matchExpr">The regular expression to match.</param>
        /// <param name="groupName">Name of the group to select.</param>
        /// <returns>The value of the group specified by <paramref name="groupName"/> and matching the regular expression specified by <paramref name="matchExpr"/>.</returns>
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

        /// <summary>
        /// Returns the first group that matches the regular expression specified.
        /// </summary>
        /// <param name="regex">The given <see cref="Regex"/> instance.</param>
        /// <param name="matchExpr">The regular expression to match.</param>
        /// <param name="groupIndex">Index position of the group to select.</param>
        /// <returns>The value of the group specified by <paramref name="groupIndex"/> and matching the regular expression specified by <paramref name="matchExpr"/>.</returns>
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

        /// <summary>
        /// Returns the values of all groups that matches the regular expression specified.
        /// </summary>
        /// <param name="regex">The given <see cref="Regex"/> instance.</param>
        /// <param name="matchExpr">The regular expression to match.</param>
        /// <param name="groupName">Name of the group to select.</param>
        /// <returns>The values of the group specified by <paramref name="groupName"/> and matching the regular expression specified by <paramref name="matchExpr"/>.</returns>
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

        /// <summary>
        /// Returns the values of all groups that matches the regular expression specified.
        /// </summary>
        /// <param name="regex">The given <see cref="Regex"/> instance.</param>
        /// <param name="matchExpr">The regular expression to match.</param>
        /// <param name="groupIndex">Index position of the group to select.</param>
        /// <returns>The values of the group specified by <paramref name="groupIndex"/> and matching the regular expression specified by <paramref name="matchExpr"/>.</returns>
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