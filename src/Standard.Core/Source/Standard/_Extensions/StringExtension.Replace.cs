using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Security;
using Standard.Core;

namespace Standard
{
    // Ordinal compare by default. In line with .NET "Foo".Replace(...)

    partial class StringExtension
    {
        // No need to implement this because .NET has already implemented:
        // Replace(this string value, string oldValue, string newValue)

        public static string Replace(this string value, string oldValue, string newValue, int count)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (value == string.Empty)
                return value;

            if (count == -1)
                return value.Replace(oldValue, newValue);
            else
                return Replace(value, oldValue, newValue, StringComparison.Ordinal, count);
        }

        public static string ReplaceIgnoreCase(this string value, string oldValue, string newValue, int count = -1)
        {
            return Replace(value, oldValue, newValue, StringComparison.OrdinalIgnoreCase, count);
        }

        public static string Replace(this string value, string oldValue, string newValue, StringComparison comparisonType, int count)
        {
            return ReplaceInternal(value, oldValue, newValue, comparisonType, -1, count);
        }

        // Replace from right

        public static string ReplaceLast(this string value, string oldValue, string newValue)
        {
            return ReplaceLast(value, oldValue, newValue, StringComparison.Ordinal, -1);
        }

        public static string ReplaceLast(this string value, string oldValue, string newValue, int count)
        {
            return ReplaceLast(value, oldValue, newValue, StringComparison.Ordinal, count);
        }

        public static string ReplaceLastIgnoreCase(this string value, string oldValue, string newValue, int count = -1)
        {
            return ReplaceLast(value, oldValue, newValue, StringComparison.OrdinalIgnoreCase, count);
        }

        public static string ReplaceLast(this string value, string oldValue, string newValue, StringComparison comparisonType, int count)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (string.IsNullOrEmpty(oldValue))
                throw new ArgumentNullException(nameof(oldValue));

            string reverseNewValue;
            if (!string.IsNullOrEmpty(newValue))
                reverseNewValue = newValue.Reverse();
            else
                reverseNewValue = null;

            return ReplaceInternal(value.Reverse(), oldValue.Reverse(), reverseNewValue, comparisonType, -1, count).Reverse();
        }

        // Internal helper

        private static string ReplaceInternal(string value, string oldValue, string newValue, StringComparison comparisonType, int bufferInitSize, int maxLoops = -1)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (value == string.Empty)
                return value;

            if (string.IsNullOrEmpty(oldValue))
                throw new ArgumentNullException(nameof(oldValue));

            if (maxLoops == 0)
                return value;

            int loopCount = 0;
            int currentIndex = 0;
            int oldValueLength = oldValue.Length;
            int nextIndex = value.IndexOf(oldValue, comparisonType);
            StringBuilder sb = new StringBuilder(
                bufferInitSize < 0 
                    ? Math.Min(4096, value.Length) 
                    : bufferInitSize);

            while ((nextIndex >= 0) && (maxLoops == -1 ? true : loopCount < maxLoops))
            {
                sb.Append(value, currentIndex, nextIndex - currentIndex);
                if (!string.IsNullOrEmpty(newValue)) 
                    sb.Append(newValue);
                currentIndex = nextIndex + oldValueLength;
                nextIndex = value.IndexOf(oldValue, currentIndex, comparisonType);

                loopCount += 1;
            }

            sb.Append(value, currentIndex, value.Length - currentIndex);

            return sb.ToString();
        }
    }
}