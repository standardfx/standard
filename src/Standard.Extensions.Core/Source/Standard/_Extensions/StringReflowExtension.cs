using System;
using System.Collections.Generic;
using System.Linq;
using Standard.Extensions.Core;

namespace Standard
{
    /// <summary>
    /// String extensions for reflowing text.
    /// </summary>
    public static class StringReflowExtension
    {
        /// <summary>
        /// Strip all newlines from the input string, then return chunks of text of at most <paramref name="maxChars"/> characters.
        /// </summary>
        /// <param name="value">String to reflow.</param>
        /// <param name="maxChars">Maximum number of characters per line.</param>
        public static IEnumerable<string> Reflow(this string value, int maxChars)
        {
            return ReflowWords(value, maxChars, null);
        }

        /// <summary>
        /// Strip all newlines from the input string, then return chunks of text of at most <paramref name="maxChars"/> characters.
        /// </summary>
        /// <param name="value">String to reflow.</param>
        /// <param name="maxChars">Maximum number of characters per line.</param>
        /// <remarks>
        /// <para>
        /// If a word (defined as consecutive characters that aren't spaces or tabs) is being split, the line will return up to the last whole word.
        /// If a word's length is longer than <paramref name="maxChars"/>, it will be forced to split across multiple lines.
        /// </para>
        /// <para>
        /// If multiple separator characters occur at a line boundary, they will be removed.
        /// </para>
        /// </remarks>
        public static IEnumerable<string> ReflowWords(this string value, int maxChars)
        {
            return ReflowWords(value, maxChars, new[] { ' ', '\t', '\0' });
        }

        /// <summary>
        /// <para>
        /// Strip all newlines from the input string, then return chunks
        /// of text of at most <paramref name="maxChars"/> characters.
        /// </para>
        /// <para>
        /// If a word (defined as consecutive characters that aren't
        /// given in <paramref name="separators"/>) is being split,
        /// the line will return up to the last whole word. If a word's
        /// length is longer than <paramref name="maxChars"/>, it will
        /// be forced to split across multiple lines.
        /// </para>
        /// <para>
        /// If multiple separator characters occur at a line boundary, they
        /// will be removed.
        /// </para>
        /// </summary>
        /// <param name="value">String to reflow.</param>
        /// <param name="maxChars">Maximum number of characters per line.</param>
        /// <param name="separators">Array of valid separator characters that prepends and/or appends each word. In English it will be the whitespace character.</param>
        /// <returns></returns>
        public static IEnumerable<string> ReflowWords(this string value, int maxChars, char[] separators)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            //bool hasSeparator = true;
            if (separators == null)
            {
                separators = new char[0];
                //hasSeparator = false;
            }

            if (maxChars < 1)
                throw new ArgumentOutOfRangeException(nameof(maxChars), RS.Err_RequireGtZero);

            // Remove newlines
            string text = value
                .Replace("\r\n", " ")
                .Replace("\n", " ");

            int start = FindNextNonSeparator(text, 0, separators);
            for (int end = start; end < text.Length; end++)
            {
                if (end - start < maxChars) 
                    continue;

                // There's a separator at the boundary
                if (separators.Contains(text[end]))
                {
                    string str = text.Substring(start, FindPreviousSeparator(text, start, end, separators) - start);

                    if (str.Length > 0)
                        yield return str;
                }
                // We're splitting a word up, step backwards to the beginning of the word.
                else
                {
                    int tmpEnd = FindPreviousNonSeparator(text, start, end, separators);

                    // The word must be longer than the line.
                    // Force a split.
                    if (tmpEnd == start)
                    {
                        string str = text.Substring(start, end - start);
                        if (str.Length > 0)
                            yield return str;
                    }
                    else
                    {
                        end = tmpEnd;
                        string str = text.Substring(start, FindPreviousSeparator(text, start, tmpEnd, separators) - start);

                        if (str.Length > 0)
                            yield return str;
                    }
                }
                start = FindNextNonSeparator(text, end, separators);
            }

            if (start != text.Length)
            {
                int end = text.Length - 1;
                int tmpEnd = FindPreviousNonSeparator(text, start, end, separators);

                if (tmpEnd == start)
                {
                    string str = text.Substring(start);
                    if (str.Length > 0)
                        yield return str;
                }
                else
                {
                    /*
                    string textWithTwoSeparators;
                    if (hasSeparator)
                        textWithTwoSeparators = text + separators[0].ToString() + separators[0].ToString();
                    else
                        textWithTwoSeparators = text;
                    */

                    //Console.WriteLine("Prev sep {0}; start {1}", FindPreviousSeparator(text, start, tmpEnd, separators), start);

                    int lastSeparator = FindPreviousSeparator(text, start, tmpEnd, separators);

                    string str = text.Length > lastSeparator 
                        ? text.Substring(start)
                        : text.Substring(start, lastSeparator - start);

                    if (str.Length > 0)
                        yield return str;
                }
            }
        }

        private static int FindNextNonSeparator(string text, int start, char[] seps)
        {
            while (start < text.Length && seps.Contains(text[start]))
            {
                start++;
            }
            return start;
        }

        private static int FindPreviousSeparator(string text, int start, int end, char[] seps)
        {
            while (end > start && seps.Contains(text[end - 1]))
            {
                end--;
            }
            return end;
        }

        private static int FindPreviousNonSeparator(string text, int start, int end, char[] seps)
        {
            // Step backwards to the beginning of the word
            while (end > start && !seps.Contains(text[end - 1]))
            {
                end--;
            }
            return end;
        }
    }
}