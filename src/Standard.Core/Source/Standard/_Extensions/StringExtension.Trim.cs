using System;
using System.Collections.Generic;
using System.Globalization;
using Standard.Core;

namespace Standard
{
    // Ordinal compare by default. In line with .NET "Foo".Trim([char[]]...)

    partial class StringExtension
    {
        // Trim

        public static string Trim(this string value, params string[] trimStrings)
        {
            return Trim(value, trimStrings, StringComparison.Ordinal);
        }

        public static string TrimIgnoreCase(this string value, params string[] trimStrings)
        {
            return Trim(value, trimStrings, StringComparison.OrdinalIgnoreCase);
        }

        public static string Trim(this string value, string[] trimStrings, StringComparison comparisonType)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value == string.Empty)
                return value;

            if (trimStrings == null || trimStrings.Length == 0)
                return value.Trim();

            int trimStartLength = 0;
            int trimEndLength = 0;
            foreach (string item in trimStrings)
            {
                if (string.IsNullOrEmpty(item))
                    continue;

                if (value.StartsWith(item, comparisonType) && (item.Length > trimStartLength))
                    trimStartLength = item.Length;

                if (value.EndsWith(item, comparisonType) && (item.Length > trimEndLength))
                    trimEndLength = item.Length;
            }

            if (trimStartLength == 0 && trimEndLength == 0)
                return value;
            else
                return value.Substring(trimStartLength, value.Length - trimEndLength - trimStartLength);
        }


        // TrimStart

        public static string TrimStart(this string value, params string[] prefix)
        {
            return TrimStart(value, prefix, StringComparison.Ordinal);
        }

        public static string TrimStartIgnoreCase(this string value, params string[] prefix)
        {
            return TrimStart(value, prefix, StringComparison.OrdinalIgnoreCase);
        }

        public static string TrimStart(this string value, string[] prefix, StringComparison comparisonType)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value == string.Empty)
                return value;

            // validate
            if (prefix == null || prefix.Length == 0)
                return value.TrimStart();

            // find the longest prefix that matches
            int trimLength = 0;
            foreach (string prefixItem in prefix)
            {
                if (string.IsNullOrEmpty(prefixItem))
                    continue;

                if (value.StartsWith(prefixItem, comparisonType) && (prefixItem.Length > trimLength))
                    trimLength = prefixItem.Length;
            }

            // clip off
            if (trimLength == 0)
                return value;
            else
                return value.Substring(trimLength);
        }


        // TrimEnd

        public static string TrimEnd(this string value, params string[] suffix)
        {
            return TrimEnd(value, suffix, StringComparison.Ordinal);
        }

        public static string TrimEndIgnoreCase(this string value, params string[] suffix)
        {
            return TrimEnd(value, suffix, StringComparison.OrdinalIgnoreCase);
        }

        public static string TrimEnd(this string value, string[] suffix, StringComparison comparisonType)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value == string.Empty)
                return value;

            if (suffix == null || suffix.Length == 0)
                return value.TrimEnd();

            int trimLength = 0;
            foreach (string suffixItem in suffix)
            {
                if (string.IsNullOrEmpty(suffixItem))
                    continue;

                if (value.EndsWith(suffixItem, comparisonType) && (suffixItem.Length > trimLength))
                    trimLength = suffixItem.Length;
            }

            if (trimLength == 0)
                return value;
            else
                return value.Substring(0, value.Length - trimLength);
        }
    }
}
