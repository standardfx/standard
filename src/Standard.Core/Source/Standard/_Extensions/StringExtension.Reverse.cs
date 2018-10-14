using System;
using System.Collections.Generic;
using System.Globalization;
using Standard.Core;

namespace Standard
{
    partial class StringExtension
    {
        /// <summary>
        /// Reverse a string. For unicode culture sensitive reversing, use the <see cref="Reverse(string, bool)"/> function.
        /// </summary>
        /// <param name="value">The string to reverse.</param>
        /// <returns>The value of <paramref name="value"/> reversed.</returns>
        public static string Reverse(this string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (value == string.Empty)
                return value;

            char[] arr = value.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        /// <summary>
        /// Reverse a string.
        /// </summary>
        /// <param name="value">The string to reverse.</param>
        /// <param name="unicode">Consider unicode characters when reversing.</param>
        /// <returns>The value of <paramref name="value"/> reversed.</returns>
        public static string Reverse(this string value, bool unicode)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value == string.Empty)
                return value;

            if (!unicode)
                return Reverse(value);

            TextElementEnumerator enumerator = StringInfo.GetTextElementEnumerator(value);
            List<string> elements = new List<string>();

            while (enumerator.MoveNext())
            {
                elements.Add(enumerator.GetTextElement());
            }

            elements.Reverse();

            return string.Concat(elements);
        }
    }
}